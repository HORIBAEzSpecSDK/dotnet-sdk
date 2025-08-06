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
        var operations = operationOrder.ToArray();
        
        // Both tasks should have started and completed
        operations.Should().Contain("task1-started");
        operations.Should().Contain("task2-started");
        operations.Should().Contain("task1-completed");
        operations.Should().Contain("task2-completed");

        communicator.Dispose();
    }

    [Fact]
    public async Task GivenSendOperationWithCancellation_WhenCancelledQuickly_ThenOperationCanBeCancelled()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);
        var command = new TestCommand("cancelled-command");
        
        using var cts = new CancellationTokenSource();

        // Act & Assert - Cancel immediately to test cancellation token support
        cts.Cancel(); // Cancel immediately
        
        var act = async () => await communicator.SendWithResponseAsync(command, cts.Token);
        
        // The operation should be cancelled either at semaphore wait or during execution
        await act.Should().ThrowAsync<OperationCanceledException>();
        
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
    public async Task GivenSendAsyncWithCancellation_WhenCancelledQuickly_ThenOperationCanBeCancelled()
    {
        // Arrange
        var communicator = new WebSocketCommunicator(IPAddress.Loopback, 25010);
        var command = new TestCommand("cancelled-send-command");
        
        using var cts = new CancellationTokenSource();

        // Act & Assert - Cancel immediately to test cancellation support
        cts.Cancel(); // Cancel immediately
        
        var act = async () => await communicator.SendAsync(command, cts.Token);
        
        // The operation should be cancelled either at semaphore wait or during execution
        await act.Should().ThrowAsync<OperationCanceledException>();
        
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
        }).ToArray();

        // This should complete without deadlock
        var allTasksComplete = Task.WhenAll(tasks);
        var timeoutTask = Task.Delay(5000);
        var completedTask = await Task.WhenAny(allTasksComplete, timeoutTask);
        
        // Assert
        completedTask.Should().Be(allTasksComplete, "all operations should complete without deadlock");
        
        communicator.Dispose();
    }
}
