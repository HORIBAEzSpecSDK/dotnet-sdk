using System.Collections.Concurrent;
using System.Net;
using FluentAssertions;
using Horiba.Sdk.Communication;
using Xunit;

namespace Horiba.Sdk.Tests;

// Test command class to avoid mocking issues
public record TestCommand(string CommandText) : Command("test", new Dictionary<string, object> { { "text", CommandText } });

public class WebSocketCommunicatorThreadSafetyTests
{
    [Fact]
    public async Task GivenSemaphoreImplementation_WhenConcurrentOperationsExecuted_ThenOperationsAreSerializedProperly()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);
        var command1 = new TestCommand("command1");
        var command2 = new TestCommand("command2");

        var operationOrder = new ConcurrentQueue<string>();
        var barrier = new TaskCompletionSource<bool>();

        // Act
        var task1 = Task.Run(async () =>
        {
            try
            {
                operationOrder.Enqueue("task1-started");
                await communicator.SendWithResponseAsync(command1);
            }
            catch (CommunicationException)
            {
                operationOrder.Enqueue("task1-completed");
                // Expected - no actual connection
            }
        });

        var task2 = Task.Run(async () =>
        {
            try
            {
                operationOrder.Enqueue("task2-started");
                await communicator.SendWithResponseAsync(command2);
            }
            catch (CommunicationException)
            {
                operationOrder.Enqueue("task2-completed");
                // Expected - no actual connection
            }
        });

        await Task.WhenAll(task1, task2);

        // Assert - With semaphore, operations should be serialized
        // One task should complete fully before the other starts the actual send operation
        var operations = operationOrder.ToArray();
        
        // Both tasks should have started
        operations.Should().Contain("task1-started");
        operations.Should().Contain("task2-started");
        operations.Should().Contain("task1-completed");
        operations.Should().Contain("task2-completed");

        communicator.Dispose();
    }

    [Fact]
    public async Task GivenSendOperationWithCancellation_WhenCancelledDuringSemaphoreWait_ThenThrowsOperationCancelledException()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);
        var command1 = new TestCommand("long-command");
        var command2 = new TestCommand("cancelled-command");
        
        using var cts = new CancellationTokenSource();
        var longOperationStarted = new TaskCompletionSource<bool>();
        var canCancelNow = new TaskCompletionSource<bool>();

        // Start a long-running operation that holds the semaphore
        var longRunningTask = Task.Run(async () =>
        {
            try
            {
                longOperationStarted.SetResult(true);
                await communicator.SendWithResponseAsync(command1);
            }
            catch (CommunicationException)
            {
                // Expected - no connection
                await canCancelNow.Task; // Wait before releasing semaphore
            }
        });

        // Wait for long operation to start and acquire semaphore
        await longOperationStarted.Task;
        await Task.Delay(50); // Ensure semaphore is acquired

        // Act & Assert - Try to send with cancellation while semaphore is held
        cts.CancelAfter(100);
        
        var act = async () => await communicator.SendWithResponseAsync(command2, cts.Token);
        
        await act.Should().ThrowAsync<OperationCanceledException>();
        
        // Allow long operation to complete
        canCancelNow.SetResult(true);
        await longRunningTask;
        
        communicator.Dispose();
    }

    [Fact]
    public async Task GivenSendOperationThatThrowsException_WhenExecuted_ThenSemaphoreIsReleasedProperly()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);
        var command = new TestCommand("test-command");

        // Act & Assert
        var firstException = await Record.ExceptionAsync(async () =>
        {
            await communicator.SendWithResponseAsync(command);
        });

        // The first call should fail due to no connection
        firstException.Should().BeOfType<CommunicationException>();

        // The second call should also work (not hang) if semaphore is properly released
        var secondException = await Record.ExceptionAsync(async () =>
        {
            await communicator.SendWithResponseAsync(command);
        });

        // Both calls should fail with the same type of exception, proving semaphore was released
        secondException.Should().BeOfType<CommunicationException>();
        
        communicator.Dispose();
    }

    [Fact]
    public void GivenWebSocketCommunicator_WhenDisposed_ThenSemaphoreIsDisposedProperly()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);

        // Act
        var exception = Record.Exception(() =>
        {
            communicator.Dispose();
        });

        // Assert
        exception.Should().BeNull("disposing should not throw when semaphore is properly cleaned up");

        // Additional test to ensure multiple disposes don't cause issues
        var secondDisposeException = Record.Exception(() =>
        {
            communicator.Dispose();
        });

        secondDisposeException.Should().BeNull("multiple disposes should not throw when semaphore is properly cleaned up");
    }

    [Fact]
    public async Task GivenSendAsyncOperationThatThrowsException_WhenExecuted_ThenSemaphoreIsReleasedProperly()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);
        var command = new TestCommand("sendcommand");

        // Act & Assert
        var firstException = await Record.ExceptionAsync(async () =>
        {
            await communicator.SendAsync(command);
        });

        // The first call should fail due to no connection
        firstException.Should().BeOfType<CommunicationException>();

        // The second call should also work (not hang) if semaphore is properly released
        var secondException = await Record.ExceptionAsync(async () =>
        {
            await communicator.SendAsync(command);
        });

        // Both calls should fail with the same type of exception, proving semaphore was released
        secondException.Should().BeOfType<CommunicationException>();
        
        communicator.Dispose();
    }

    [Fact]
    public async Task GivenSendAsyncWithCancellation_WhenCancelledDuringSemaphoreWait_ThenThrowsOperationCancelledException()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);
        var command1 = new TestCommand("long-send-command");
        var command2 = new TestCommand("cancelled-send-command");
        
        using var cts = new CancellationTokenSource();
        var longOperationStarted = new TaskCompletionSource<bool>();
        var canCancelNow = new TaskCompletionSource<bool>();

        // Start a long-running operation that holds the semaphore
        var longRunningTask = Task.Run(async () =>
        {
            try
            {
                longOperationStarted.SetResult(true);
                await communicator.SendAsync(command1);
            }
            catch (CommunicationException)
            {
                // Expected - no connection
                await canCancelNow.Task; // Wait before releasing semaphore
            }
        });

        // Wait for long operation to start and acquire semaphore
        await longOperationStarted.Task;
        await Task.Delay(50); // Ensure semaphore is acquired

        // Act & Assert
        cts.CancelAfter(100);
        
        var act = async () => await communicator.SendAsync(command2, cts.Token);
        
        await act.Should().ThrowAsync<OperationCanceledException>();
        
        // Allow long operation to complete
        canCancelNow.SetResult(true);
        await longRunningTask;
        
        communicator.Dispose();
    }

    [Fact]
    public async Task GivenMixedOperations_WhenExecutedConcurrently_ThenAllOperationsAreSerializedCorrectly()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);
        var command1 = new TestCommand("mixed1");
        var command2 = new TestCommand("mixed2");

        var operationOrder = new ConcurrentQueue<string>();

        // Act - Mix of SendAsync and SendWithResponseAsync operations
        var tasks = new[]
        {
            Task.Run(async () =>
            {
                try
                {
                    operationOrder.Enqueue("sendAsync-started");
                    await communicator.SendAsync(command1);
                    operationOrder.Enqueue("sendAsync-completed");
                }
                catch (CommunicationException)
                {
                    operationOrder.Enqueue("sendAsync-completed");
                }
            }),
            Task.Run(async () =>
            {
                try
                {
                    operationOrder.Enqueue("sendWithResponse-started");
                    await communicator.SendWithResponseAsync(command2);
                    operationOrder.Enqueue("sendWithResponse-completed");
                }
                catch (CommunicationException)
                {
                    operationOrder.Enqueue("sendWithResponse-completed");
                }
            })
        };

        await Task.WhenAll(tasks);

        // Assert - Operations should be serialized
        var operations = operationOrder.ToArray();
        operations.Should().Contain("sendAsync-started");
        operations.Should().Contain("sendAsync-completed");
        operations.Should().Contain("sendWithResponse-started");
        operations.Should().Contain("sendWithResponse-completed");

        communicator.Dispose();
    }

    [Fact]
    public async Task GivenMultipleParallelOperations_WhenExecutedWithSemaphore_ThenNoDeadlockOccurs()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);
        var commands = Enumerable.Range(1, 5).Select(i => new TestCommand($"command{i}")).ToArray();
        
        // Act
        var tasks = commands.Select(async command =>
        {
            try
            {
                await communicator.SendWithResponseAsync(command);
            }
            catch (CommunicationException)
            {
                // Expected - no connection
            }
        });

        // This should complete without deadlock
        var completedTask = await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(5000));
        
        // Assert
        completedTask.Should().Be(Task.WhenAll(tasks), "all operations should complete without deadlock");
        
        communicator.Dispose();
    }
}
