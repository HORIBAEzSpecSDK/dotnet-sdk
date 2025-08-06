# API Reference

Welcome to the HORIBA EzSpec SDK API Reference. This section contains detailed documentation for all classes, interfaces, and methods in the SDK.

## Core Namespaces

### Horiba.Sdk.Devices
Contains the main device classes for controlling HORIBA instruments:
- `DeviceManager` - Central hub for device discovery and management
- `MonochromatorDevice` - Control monochromators and spectrometers
- `ChargedCoupledDevice` - Control CCD cameras and detectors
- `SpectrAcqDevice` - Interface with SpectrAcq software

### Horiba.Sdk.Communication
Low-level communication classes for WebSocket connectivity:
- `WebSocketCommunicator` - Handles WebSocket communication with ICL
- `Command` - Base class for all device commands
- `Response` - Represents responses from devices

### Horiba.Sdk.Data
Data structures for spectroscopic measurements:
- `CcdData` - Represents CCD acquisition data
- `SaqData` - SpectrAcq measurement data
- `RegionOfInterestDescription` - ROI configuration

### Horiba.Sdk.Enums
Enumerations for device parameters:
- `Grating` - Grating positions for monochromators
- `Mirror` - Mirror positions and types
- `Slit` - Slit identification and control
- `ConversionType` - X-axis conversion types

## Getting Started

For usage examples and tutorials, see the [Getting Started Guide](../articles/getting-started.md).

## Class Hierarchy

Browse the API documentation by namespace or use the search functionality to find specific classes and methods.
