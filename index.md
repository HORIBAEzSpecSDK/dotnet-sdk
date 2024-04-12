# horiba-dotnet-sdk
This is the c# .NET repository for the Horiba SDK components.

___

⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️⬇️

> [!WARNING]  
> This SDK is under development and not yet released.

> [!IMPORTANT]  
> For this .NET code to work, the SDK from Horiba has to be purchased, installed and licensed.
> The code in this repo and the SDK are under development and not yet released for public use!

⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️⬆️

___

**📦 Prerequisites**

* .NET Standard or .NET 6+
* ICL.exe installed as part of the Horiba SDK, licensed and activated





# Command Status

## ICL version 2.0.0.124.b4cc4e55
---
---

# Generic Commands

| Command      | Implemented | Tests available | Status | Comment |
| ------------ | :---------: | :-------------: | :----: | :-----: |
| icl_info     |      ✅      |        ✖️        |   ✖️    |         |
| icl_shutdown |      ✅      |        ✖️        |   ✖️    |         |
| icl_binMode  |      ✅      |        ✖️        |   ✖️    |         |

# Monochromator Commands

| Command                     | Implemented | Tests available | Status |                                          Comment                                          |
| --------------------------- | :---------: | :-------------: | :----: | :---------------------------------------------------------------------------------------: |
| mono_discover               |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_list                   |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_listCount              |      ✖️      |        ✖️        |   ✖️    |                                                                                           |
| mono_open                   |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_close                  |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_isOpen                 |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_isBusy                 |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_init                   |      ✅      |        ✅        |   ⚠️    | There is no way to reliably understand if a homing cycle was done after the last power-up |
| mono_getConfig              |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getPosition            |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_setPosition            |      ✅      |        ✖️        |   ⚠️    |                        This operation will un-calibrate the device                        |
| mono_moveToPosition         |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getGratingPosition     |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveGrating            |      ✅      |        ✅        |   ⛔    |                     Grating does not move even after 30s waiting time                     |
| mono_getFilterWheelPosition |      ✅      |        ✅        |   ⚠️    |                                                                                           |
| mono_moveFilterWheel        |      ✅      |        ✅        |   ⛔    |                         [E];-510;Error Mono Command Not Supported                         |
| mono_getMirrorPosition      |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveMirror             |      ✅      |        ✅        |   ⛔    |                        First and Second mirror cannot move literal                        |
| mono_getSlitPositionInMM    |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveSlitMM             |      ✅      |        ✅        |   ⛔    |                                   Slit C is not moving                                    |
| mono_shutterSelect          |      ✖️      |        ✖️        |   ✖️    |                                                                                           |
| mono_shutterOpen            |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_shutterClose           |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_getShutterStatus       |      ✅      |        ✅        |   ⚠️    |                                                                                           |
| mono_getSlitStepPosition    |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveSlit               |      ✅      |        ✅        |   ⛔    |                                   Slit C is not moving                                    |

# CCD Commands

| Command                    | Implemented | Tests available | Status |                                            Comment                                            |
| -------------------------- | :---------: | :-------------: | :----: | :-------------------------------------------------------------------------------------------: |
| ccd_discover               |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_list                   |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_listCount              |      ✖️      |        ✖️        |   ✖️    |                                                                                               |
| ccd_open                   |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_close                  |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_isOpen                 |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_restart                |      ✅      |        ✖️        |   ❓    |                            how can this be tested with code only?                             |
| ccd_getConfig              |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_getChipSize            |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_getChipTemperature     |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_getNumberOfAvgs        |      ✅      |        ✅        |   ⚠️    | If we cannot set the parameter, how can we test the readout operation for the same parameter? |
| ccd_setNumberOfAvgs        |      ✅      |        ✅        |   ⛔    |                            [E];-315;CCD does not support averaging                            |
| ccd_getGain                |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_setGain                |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_getSpeed               |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_setSpeed               |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_getFitParams           |      ✅      |        ✅        |   ⚠️    | If we cannot set the parameter, how can we test the readout operation for the same parameter? |
| ccd_setFitParams           |      ✅      |        ✅        |   ⛔    |                              how should these parameters be used                              |
| ccd_getExposureTime        |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_setExposureTime        |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_getTimerResolution     |      ✅      |        ✅        |   ⚠️    |                                                                                               |
| ccd_setTimerResolution     |      ✅      |        ✅        |   ⛔    |                             when set to 500 or 5000, returns 1000                             |
| ccd_setAcqFormat           |      ✅      |        ✖️        |   ❓    |                            how can this be tested with code only?                             |
| ccd_setRoi                 |      ✅      |        ✖️        |   ❓    |                            how can this be tested with code only?                             |
| ccd_getXAxisConversionType |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_setXAxisConversionType |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_getDataRetrievalMethod |      ✖️      |        ✖️        |   ✖️    |                                                                                               |
| ccd_setDataRetrievalMethod |      ✖️      |        ✖️        |   ✖️    |                                                                                               |
| ccd_getAcqCount            |      ✅      |        ✅        |   ⚠️    |                                                                                               |
| ccd_setAcqCount            |      ✅      |        ✅        |   ⛔    |                                  when set to 5, returns 1000                                  |
| ccd_getCleanCount          |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_setCleanCount          |      ✅      |        ✅        |   ✅    |                                                                                               |
| ccd_getDataSize            |      ✅      |        ✖️        |   ✖️    |                                                                                               |
| ccd_getTriggerIn           |      ✖️      |        ✖️        |   ✖️    |                                                                                               |
| ccd_setTriggerIn           |      ✖️      |        ✖️        |   ✖️    |                                                                                               |
| ccd_getSignalOut           |      ✖️      |        ✖️        |   ✖️    |                                                                                               |
| ccd_setSignalOut           |      ✖️      |        ✖️        |   ✖️    |                                                                                               |
| ccd_getAcquisitionReady    |      ✅      |        ✅        |   ⚠️    |                                                                                               |
| ccd_setAcquisitionStart    |      ✅      |        ✅        |   ⚠️    |                                                                                               |
| ccd_getAcquisitionBusy     |      ✅      |        ✅        |   ⚠️    |                                                                                               |
| ccd_setAcquisitionAbort    |      ✅      |        ✖️        |   ❓    |                            what is the expected 'Abort' procedure?                            |
| ccd_getAcquisitionData     |      ✅      |        ✅        |   ⛔    |                             [E];-326;Error Data Formatting Error                              |

# Single Chanel Detector Commands

| Command       | Implemented | Tested | Status | Comment |
| ------------- | :---------: | -----: | -----: | ------: |
| scd_discover  |      ✖️      |      ✖️ |      ✖️ |         |
| scd_list      |      ✖️      |      ✖️ |      ✖️ |         |
| scd_listCount |      ✖️      |      ✖️ |      ✖️ |         |
| scd_open      |      ✖️      |      ✖️ |      ✖️ |         |
| scd_close     |      ✖️      |      ✖️ |      ✖️ |         |
| scd_isOpen    |      ✖️      |      ✖️ |      ✖️ |         |


---
---