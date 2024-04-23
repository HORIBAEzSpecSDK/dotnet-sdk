# Command Status

---
---

## ICL version 2.0.0.226ac9f0_126_20240417

**Generic Commands**

| Command      | Implemented | Tests available | Status |                       Comment                        |
| ------------ | :---------: | :-------------: | :----: | :--------------------------------------------------: |
| icl_info     |      ✅      |        ✅        |   ✅    |                                                      |
| icl_shutdown |      ✅      |        ✅        |   ✅    |                                                      |
| icl_binMode  |      ✅      |        ❓        |   ✖️    | how will this command affect the work of the devices |

**Monochromator Commands**

| Command                     | Implemented | Tests available | Status |                                          Comment                                          |
| --------------------------- | :---------: | :-------------: | :----: | :---------------------------------------------------------------------------------------: |
| mono_discover               |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_list                   |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_listCount              |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_open                   |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_close                  |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_isOpen                 |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_isBusy                 |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_init                   |      ✅      |        ✅        |   ⚠️    | there is no way to reliably understand if a homing cycle was done after the last power-up |
| mono_getConfig              |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getPosition            |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_setPosition            |      ✅      |        ✖️        |   ⚠️    |                        this operation will un-calibrate the device                        |
| mono_moveToPosition         |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getGratingPosition     |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveGrating            |      ✅      |        ✅        |   ✅    |                              we have to wait 2 mins per test                              |
| mono_getFilterWheelPosition |      ✅      |        ✅        |   ⛔    |                         [E];-510;Error Mono Command Not Supported                         |
| mono_moveFilterWheel        |      ✅      |        ✅        |   ⛔    |                         [E];-510;Error Mono Command Not Supported                         |
| mono_getMirrorPosition      |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveMirror             |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getSlitPositionInMM    |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveSlitMM             |      ✅      |        ✅        |   ⚠️    |                                   Slit C is not moving                                    |
| mono_shutterSelect          |      ✅      |        ✅        |   ⚠️    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_shutterOpen            |      ✅      |        ✅        |   ⚠️    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_shutterClose           |      ✅      |        ✅        |   ⚠️    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_getShutterStatus       |      ✅      |        ✅        |   ⚠️    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_getSlitStepPosition    |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveSlit               |      ✅      |        ✅        |   ✅    |                                   Slit C is not moving                                    |

**CCD Commands**

| Command                    | Implemented | Tests available | Status |                            Comment                            |
| -------------------------- | :---------: | :-------------: | :----: | :-----------------------------------------------------------: |
| ccd_discover               |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_list                   |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_listCount              |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_open                   |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_close                  |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_isOpen                 |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_restart                |      ✅      |        ✖️        |   ✅    |                                                               |
| ccd_getConfig              |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getChipSize            |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getChipTemperature     |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getGain                |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setGain                |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getSpeed               |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setSpeed               |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getFitParams           |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setFitParams           |      ✅      |        ✅        |   ⛔    |            how should these parameters be used/set            |
| ccd_getExposureTime        |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setExposureTime        |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getTimerResolution     |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setTimerResolution     |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setAcqFormat           |      ✅      |        ✖️        |   ❓    |            how can this be tested with code only?             |
| ccd_setRoi                 |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getXAxisConversionType |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setXAxisConversionType |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getDataRetrievalMethod |      ⛔      |        ✖️        |   ✖️    | "[E];-2;ccd_getDataRetrievalMethod;Command handler not found" |
| ccd_setDataRetrievalMethod |      ⛔      |        ✖️        |   ✖️    | "[E];-2;ccd_getDataRetrievalMethod;Command handler not found" |
| ccd_getAcqCount            |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setAcqCount            |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getCleanCount          |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setCleanCount          |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getDataSize            |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getTriggerIn           |      ✖️      |        ✖️        |   ✖️    |                                                               |
| ccd_setTriggerIn           |      ✖️      |        ✖️        |   ✖️    |                                                               |
| ccd_getSignalOut           |      ✖️      |        ✖️        |   ✖️    |                                                               |
| ccd_setSignalOut           |      ✖️      |        ✖️        |   ✖️    |                                                               |
| ccd_getAcquisitionReady    |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setAcquisitionStart    |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_getAcquisitionBusy     |      ✅      |        ✅        |   ✅    |                                                               |
| ccd_setAcquisitionAbort    |      ✅      |        ✖️        |   ❓    |            what is the expected 'Abort' procedure?            |
| ccd_getAcquisitionData     |      ✅      |        ✅        |   ✅    |                                                               |

**Single Chanel Detector Commands**

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

## ICL version 2.0.0.125.45db7b8c

**Generic Commands**

| Command      | Implemented | Tests available | Status |                       Comment                        |
| ------------ | :---------: | :-------------: | :----: | :--------------------------------------------------: |
| icl_info     |      ✅      |        ✅        |   ✅    |                                                      |
| icl_shutdown |      ✅      |        ✅        |   ✅    |                                                      |
| icl_binMode  |      ✅      |        ❓        |   ✖️    | how will this command affect the work of the devices |

**Monochromator Commands**

| Command                     | Implemented | Tests available | Status |                                          Comment                                          |
| --------------------------- | :---------: | :-------------: | :----: | :---------------------------------------------------------------------------------------: |
| mono_discover               |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_list                   |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_listCount              |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_open                   |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_close                  |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_isOpen                 |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_isBusy                 |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_init                   |      ✅      |        ✅        |   ⚠️    | there is no way to reliably understand if a homing cycle was done after the last power-up |
| mono_getConfig              |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getPosition            |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_setPosition            |      ✅      |        ✖️        |   ⚠️    |                        this operation will un-calibrate the device                        |
| mono_moveToPosition         |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getGratingPosition     |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveGrating            |      ✅      |        ✅        |   ✅    |                              we have to wait 2 mins per test                              |
| mono_getFilterWheelPosition |      ✅      |        ✅        |   ⛔    |                         [E];-510;Error Mono Command Not Supported                         |
| mono_moveFilterWheel        |      ✅      |        ✅        |   ⛔    |                         [E];-510;Error Mono Command Not Supported                         |
| mono_getMirrorPosition      |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveMirror             |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getSlitPositionInMM    |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveSlitMM             |      ✅      |        ✅        |   ⚠️    |                                   Slit C is not moving                                    |
| mono_shutterSelect          |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_shutterOpen            |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_shutterClose           |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_getShutterStatus       |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_getSlitStepPosition    |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveSlit               |      ✅      |        ✅        |   ⚠️    |                                   Slit C is not moving                                    |

**CCD Commands**

| Command                    | Implemented | Tests available | Status |                          Comment                           |
| -------------------------- | :---------: | :-------------: | :----: | :--------------------------------------------------------: |
| ccd_discover               |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_list                   |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_listCount              |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_open                   |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_close                  |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_isOpen                 |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_restart                |      ✅      |        ✖️        |   ✅    |                                                            |
| ccd_getConfig              |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getChipSize            |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getChipTemperature     |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getGain                |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setGain                |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getSpeed               |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setSpeed               |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getFitParams           |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setFitParams           |      ✅      |        ✅        |   ⛔    |          how should these parameters be used/set           |
| ccd_getExposureTime        |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setExposureTime        |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getTimerResolution     |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setTimerResolution     |      ✅      |        ✅        |   ⛔    | [E];-316;CCD does not support Timer Resolution Token = 500 |
| ccd_setAcqFormat           |      ✅      |        ✖️        |   ❓    |           how can this be tested with code only?           |
| ccd_setRoi                 |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getXAxisConversionType |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setXAxisConversionType |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getDataRetrievalMethod |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_setDataRetrievalMethod |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_getAcqCount            |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setAcqCount            |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getCleanCount          |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setCleanCount          |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getDataSize            |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getTriggerIn           |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_setTriggerIn           |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_getSignalOut           |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_setSignalOut           |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_getAcquisitionReady    |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setAcquisitionStart    |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getAcquisitionBusy     |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setAcquisitionAbort    |      ✅      |        ✖️        |   ❓    |          what is the expected 'Abort' procedure?           |
| ccd_getAcquisitionData     |      ✅      |        ✅        |   ✅    |                                                            |

**Single Chanel Detector Commands**

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

## ICL version 2.0.0.124.b4cc4e55

**Generic Commands**

| Command      | Implemented | Tests available | Status |                       Comment                        |
| ------------ | :---------: | :-------------: | :----: | :--------------------------------------------------: |
| icl_info     |      ✅      |        ✅        |   ✅    |                                                      |
| icl_shutdown |      ✅      |        ✅        |   ✅    |                                                      |
| icl_binMode  |      ✅      |        ❓        |   ✖️    | how will this command affect the work of the devices |

**Monochromator Commands**

| Command                     | Implemented | Tests available | Status |                                          Comment                                          |
| --------------------------- | :---------: | :-------------: | :----: | :---------------------------------------------------------------------------------------: |
| mono_discover               |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_list                   |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_listCount              |      ❓      |        ✖️        |   ✖️    |                         how is this different from mono_discover                          |
| mono_open                   |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_close                  |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_isOpen                 |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_isBusy                 |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_init                   |      ✅      |        ✅        |   ⚠️    | there is no way to reliably understand if a homing cycle was done after the last power-up |
| mono_getConfig              |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getPosition            |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_setPosition            |      ✅      |        ✖️        |   ⚠️    |                        this operation will un-calibrate the device                        |
| mono_moveToPosition         |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getGratingPosition     |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveGrating            |      ✅      |        ✅        |   ✅    |                              we have to wait 2 mins per test                              |
| mono_getFilterWheelPosition |      ✅      |        ✅        |   ⛔    |                         [E];-510;Error Mono Command Not Supported                         |
| mono_moveFilterWheel        |      ✅      |        ✅        |   ⛔    |                         [E];-510;Error Mono Command Not Supported                         |
| mono_getMirrorPosition      |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveMirror             |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_getSlitPositionInMM    |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveSlitMM             |      ✅      |        ✅        |   ✅    |                                   Slit C is not moving                                    |
| mono_shutterSelect          |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_shutterOpen            |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_shutterClose           |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_getShutterStatus       |      ✅      |        ✅        |   ⛔    |                [E];-519;Mono must be configured for internal shutter mode                 |
| mono_getSlitStepPosition    |      ✅      |        ✅        |   ✅    |                                                                                           |
| mono_moveSlit               |      ✅      |        ✅        |   ✅    |                                   Slit C is not moving                                    |

**CCD Commands**

| Command                    | Implemented | Tests available | Status |                          Comment                           |
| -------------------------- | :---------: | :-------------: | :----: | :--------------------------------------------------------: |
| ccd_discover               |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_list                   |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_listCount              |      ❓      |        ✖️        |   ✖️    |           how is this different fromccd_discover           |
| ccd_open                   |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_close                  |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_isOpen                 |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_restart                |      ✅      |        ✖️        |   ❓    |           how can this be tested with code only?           |
| ccd_getConfig              |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getChipSize            |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getChipTemperature     |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getNumberOfAvgs        |      ✅      |        ✅        |   ⛔    |          [E];-315;CCD does not support averaging           |
| ccd_setNumberOfAvgs        |      ✅      |        ✅        |   ⛔    |          [E];-315;CCD does not support averaging           |
| ccd_getGain                |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setGain                |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getSpeed               |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setSpeed               |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getFitParams           |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setFitParams           |      ✅      |        ✅        |   ⛔    |          how should these parameters be used/set           |
| ccd_getExposureTime        |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setExposureTime        |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getTimerResolution     |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setTimerResolution     |      ✅      |        ✅        |   ⛔    | [E];-316;CCD does not support Timer Resolution Token = 500 |
| ccd_setAcqFormat           |      ✅      |        ✖️        |   ❓    |           how can this be tested with code only?           |
| ccd_setRoi                 |      ✅      |        ✖️        |   ❓    |           how can this be tested with code only?           |
| ccd_getXAxisConversionType |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setXAxisConversionType |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getDataRetrievalMethod |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_setDataRetrievalMethod |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_getAcqCount            |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setAcqCount            |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getCleanCount          |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setCleanCount          |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getDataSize            |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getTriggerIn           |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_setTriggerIn           |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_getSignalOut           |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_setSignalOut           |      ✖️      |        ✖️        |   ✖️    |                                                            |
| ccd_getAcquisitionReady    |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setAcquisitionStart    |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_getAcquisitionBusy     |      ✅      |        ✅        |   ✅    |                                                            |
| ccd_setAcquisitionAbort    |      ✅      |        ✖️        |   ❓    |          what is the expected 'Abort' procedure?           |
| ccd_getAcquisitionData     |      ✅      |        ✅        |   ⛔    |            [E];-326;Error Data Formatting Error            |

**Single Chanel Detector Commands**

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
