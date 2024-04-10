# Generic Commands

| Command      | Implemented | Tests available | Status | Comment |
| ------------ | :---------: | :-------------: | :----: | :-----: |
| icl_info     |      ✅      |        ⛔        |   ⛔    |         |
| icl_shutdown |      ✅      |        ⛔        |   ⚠️    |         |
| icl_binMode  |      ✅      |        ⛔        |   ⛔    |         |

# Monochromator Commands

| Command                     | Implemented | Tests available | Status |                              Comment                               |
| --------------------------- | :---------: | :-------------: | :----: | :----------------------------------------------------------------: |
| mono_discover               |      ✅      |        ✅        |   ✅    |                                                                    |
| mono_list                   |      ✅      |        ✅        |   ✅    |                                                                    |
| mono_listCount              |      ⛔      |        ⛔        |   ⛔    |                                                                    |
| mono_open                   |      ✅      |        ✅        |   ✅    |                                                                    |
| mono_close                  |      ✅      |        ✅        |   ✅    |                                                                    |
| mono_isOpen                 |      ✅      |        ✅        |   ⛔    |                                                                    |
| mono_isBusy                 |      ✅      |        ✅        |   ⛔    |                                                                    |
| mono_init                   |      ✅      |        ⛔        |   ⛔    |                                                                    |
| mono_getConfig              |      ✅      |        ✅        |   ✅    |                                                                    |
| mono_getPosition            |      ✅      |        ✅        |   ✅    |                                                                    |
| mono_setPosition            |      ✅      |        ⛔        |   ⚠️    |            This operation will un-calibrate the device             |
| mono_moveToPosition         |      ✅      |        ✅        |   ✅    |                                                                    |
| mono_getGratingPosition     |      ✅      |        ✅        |   ⚠️    |                                                                    |
| mono_moveGrating            |      ✅      |        ✅        |   ⛔    |                           does not work                            |
| mono_getFilterWheelPosition |      ✅      |        ✅        |   ⚠️    |                                                                    |
| mono_moveFilterWheel        |      ✅      |        ✅        |   ⛔    |             [E];-510;Error Mono Command Not Supported              |
| mono_getMirrorPosition      |      ✅      |        ✅        |   ⚠️    |                                                                    |
| mono_moveMirror             |      ✅      |        ✅        |   ⛔    |                 Second mirror cannot move literal                  |
| mono_getSlitPositionInMM    |      ✅      |        ✅        |   ⚠️    |                                                                    |
| mono_moveSlitMM             |      ✅      |        ✅        |   ⛔    | does not work, sometimes the device gets disconnected from USB bus |
| mono_shutterSelect          |      ⛔      |        ⛔        |   ⛔    |                                                                    |
| mono_shutterOpen            |      ✅      |        ✅        |   ⚠️    |     [E];-519;Mono must be configured for internal shutter mode     |
| mono_shutterClose           |      ✅      |        ✅        |   ⚠️    |     [E];-519;Mono must be configured for internal shutter mode     |
| mono_getShutterStatus       |      ✅      |        ✅        |   ⛔    |                                                                    |
| mono_getSlitStepPosition    |      ✅      |        ✅        |   ⚠️    |                                                                    |
| mono_moveSlit               |      ✅      |        ✅        |   ⛔    | does not work, sometimes the device gets disconnected from USB bus |

# CCD Commands

| Command                    | Implemented | Tests available | Status |                 Comment                 |
| -------------------------- | :---------: | :-------------: | :----: | :-------------------------------------: |
| ccd_discover               |      ✅      |        ✅        |   ✅    |                                         |
| ccd_list                   |      ✅      |        ✅        |   ✅    |                                         |
| ccd_listCount              |      ⛔      |        ⛔        |   ⛔    |                                         |
| ccd_open                   |      ✅      |        ✅        |   ✅    |                                         |
| ccd_close                  |      ✅      |        ✅        |   ✅    |                                         |
| ccd_isOpen                 |      ✅      |        ✅        |   ✅    |                                         |
| ccd_restart                |      ✅      |        ✖️        |   ✖️    |         how can this be tested          |
| ccd_getConfig              |      ✅      |        ✅        |   ✅    |                                         |
| ccd_getChipSize            |      ✅      |        ✅        |   ✅    |                                         |
| ccd_getChipTemperature     |      ✅      |        ✅        |   ✅    |                                         |
| ccd_getNumberOfAvgs        |      ✅      |        ✅        |   ⛔    | [E];-315;CCD does not support averaging |
| ccd_setNumberOfAvgs        |      ✅      |        ✅        |   ⛔    | [E];-315;CCD does not support averaging |
| ccd_getGain                |      ✅      |        ✅        |   ✅    |                                         |
| ccd_setGain                |      ✅      |        ✅        |   ✅    |                                         |
| ccd_getSpeed               |      ✅      |        ✅        |   ✅    |                                         |
| ccd_setSpeed               |      ✅      |        ✅        |   ✅    |                                         |
| ccd_getFitParams           |      ✅      |        ✅        |   ⛔    |   how should these parameters be used   |
| ccd_setFitParams           |      ✅      |        ✅        |   ⛔    |                                         |
| ccd_getExposureTime        |      ✅      |        ✅        |   ✅    |                                         |
| ccd_setExposureTime        |      ✅      |        ✅        |   ✅    |                                         |
| ccd_getTimerResolution     |      ✅      |        ✅        |   ⛔    |                                         |
| ccd_setTimerResolution     |      ✅      |        ✅        |   ⛔    |  when set to 500 or 5000, returns 1000  |
| ccd_setAcqFormat           |      ✅      |        ⛔        |   ⛔    |                                         |
| ccd_setRoi                 |      ✅      |        ⛔        |   ⛔    |                                         |
| ccd_getXAxisConversionType |      ✅      |        ✅        |   ✅    |                                         |
| ccd_setXAxisConversionType |      ✅      |        ✅        |   ✅    |                                         |
| ccd_getDataRetrievalMethod |      ⛔      |        ✖️        |   ✖️    |                                         |
| ccd_setDataRetrievalMethod |      ⛔      |        ✖️        |   ✖️    |                                         |
| ccd_getAcqCount            |      ✅      |        ✅        |   ⛔    |                                         |
| ccd_setAcqCount            |      ✅      |        ✅        |   ⛔    |       when set to 5, returns 1000       |
| ccd_getCleanCount          |      ✅      |        ✅        |   ✅    |                                         |
| ccd_setCleanCount          |      ✅      |        ✅        |   ✅    |                                         |
| ccd_getDataSize            |      ✅      |        ⛔        |   ⛔    |                                         |
| ccd_getTriggerIn           |      ⛔      |        ✖️        |   ✖️    |                                         |
| ccd_setTriggerIn           |      ⛔      |        ✖️        |   ✖️    |                                         |
| ccd_getSignalOut           |      ⛔      |        ✖️        |   ✖️    |                                         |
| ccd_setSignalOut           |      ⛔      |        ✖️        |   ✖️    |                                         |
| ccd_getAcquisitionReady    |      ✅      |        ✅        |   ⛔    |                                         |
| ccd_setAcquisitionStart    |      ✅      |        ✅        |   ⛔    |                                         |
| ccd_getAcquisitionBusy     |      ✅      |        ✅        |   ⛔    |                                         |
| ccd_setAcquisitionAbort    |      ✅      |        ✖️        |   ✖️    |                                         |
| ccd_getAcquisitionData     |      ✅      |        ✅        |   ⛔    |                                         |

# Single Chanel Detector Commands

| Command       | Implemented | Tested | Status | Comment |
| ------------- | :---------: | -----: | -----: | ------: |
| scd_discover  |      ✖️      |      ✖️ |      ✖️ |         |
| scd_list      |      ✖️      |      ✖️ |      ✖️ |         |
| scd_listCount |      ✖️      |      ✖️ |      ✖️ |         |
| scd_open      |      ✖️      |      ✖️ |      ✖️ |         |
| scd_close     |      ✖️      |      ✖️ |      ✖️ |         |
| scd_isOpen    |      ✖️      |      ✖️ |      ✖️ |         |
