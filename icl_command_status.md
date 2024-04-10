# Generic Commands

| Command      | Implemented | Tests available | Status | Comment |
| ------------ | :---------: | :-------------: | :----: | :-----: |
| icl_info     |      ✅      |        ⛔        |   ⛔    |         |
| icl_shutdown |      ✅      |        ⛔        |   ⚠️    |         |
| icl_binMode  |      ✅      |        ⛔        |   ⛔    |         |

# Monochromator Commands

| Command                     | Implemented | Tests available | Status |                   Comment                   |
| --------------------------- | :---------: | :-------------: | :----: | :-----------------------------------------: |
| mono_discover               |      ✅      |        ✅        |   ⛔    |                                             |
| mono_list                   |      ✅      |        ✅        |   ⛔    |                                             |
| mono_listCount              |      ⛔      |        ⛔        |   ⛔    |                                             |
| mono_open                   |      ✅      |        ✅        |   ⛔    |                                             |
| mono_close                  |      ✅      |        ✅        |   ⛔    |                                             |
| mono_isOpen                 |      ✅      |        ✅        |   ⛔    |                                             |
| mono_isBusy                 |      ✅      |        ✅        |   ⛔    |                                             |
| mono_init                   |      ✅      |        ⛔        |   ⛔    |                                             |
| mono_getConfig              |      ✅      |        ✅        |   ⛔    |                                             |
| mono_getPosition            |      ✅      |        ✅        |   ⛔    |                                             |
| mono_setPosition            |      ✅      |        ⛔        |   ⛔    | This operation will un-calibrate the device |
| mono_moveToPosition         |      ✅      |        ✅        |   ⛔    |                                             |
| mono_getGratingPosition     |      ✅      |        ✅        |   ⛔    |                                             |
| mono_moveGrating            |      ✅      |        ✅        |   ⛔    |                                             |
| mono_getFilterWheelPosition |      ✅      |        ✅        |   ⛔    |                                             |
| mono_moveFilterWheel        |      ✅      |        ✅        |   ⛔    |                                             |
| mono_getMirrorPosition      |      ✅      |        ✅        |   ⛔    |                                             |
| mono_moveMirror             |      ✅      |        ✅        |   ⛔    |                                             |
| mono_getSlitPositionInMM    |      ✅      |        ✅        |   ⛔    |                                             |
| mono_moveSlitMM             |      ✅      |        ✅        |   ⛔    |                                             |
| mono_shutterSelect          |      ⛔      |        ⛔        |   ⚠️    |                                             |
| mono_shutterOpen            |      ✅      |        ✅        |   ⚠️    |                                             |
| mono_shutterClose           |      ✅      |        ✅        |   ⚠️    |                                             |
| mono_getShutterStatus       |      ✅      |        ✅        |   ⚠️    |                                             |
| mono_getSlitStepPosition    |      ✅      |        ✅        |   ⛔    |                                             |
| mono_moveSlit               |      ✅      |        ✅        |   ⛔    |                                             |

# CCD Commands

| Command                    | Implemented | Tests available | Status | Comment |
| -------------------------- | :---------: | :-------------: | :----: | :-----: |
| ccd_discover               |      ✅      |        ✅        |   ⛔    |         |
| ccd_list                   |      ✅      |        ✅        |   ⛔    |         |
| ccd_listCount              |      ⛔      |        ⛔        |   ⛔    |         |
| ccd_open                   |      ✅      |        ✅        |   ⛔    |         |
| ccd_close                  |      ✅      |        ✅        |   ⛔    |         |
| ccd_isOpen                 |      ✅      |        ✅        |   ⛔    |         |
| ccd_restart                |      ✅      |        ⛔        |   ⛔    |         |
| ccd_getConfig              |      ✅      |        ✅        |   ⛔    |         |
| ccd_getChipSize            |      ✅      |        ✅        |   ⛔    |         |
| ccd_getChipTemperature     |      ✅      |        ✅        |   ⛔    |         |
| ccd_getNumberOfAvgs        |      ✅      |        ✅        |   ⛔    |         |
| ccd_setNumberOfAvgs        |      ✅      |        ✅        |   ⛔    |         |
| ccd_getGain                |      ✅      |        ✅        |   ⛔    |         |
| ccd_setGain                |      ✅      |        ✅        |   ⛔    |         |
| ccd_getSpeed               |      ✅      |        ✅        |   ⛔    |         |
| ccd_setSpeed               |      ✅      |        ✅        |   ⚠️    |         |
| ccd_getFitParams           |      ✅      |        ✅        |   ⛔    |         |
| ccd_setFitParams           |      ✅      |        ✅        |   ⚠️    |         |
| ccd_getExposureTime        |      ✅      |        ✅        |   ⛔    |         |
| ccd_setExposureTime        |      ✅      |        ✅        |   ⛔    |         |
| ccd_getTimerResolution     |      ✅      |        ✅        |   ⛔    |         |
| ccd_setTimerResolution     |      ✅      |        ✅        |   ⛔    |         |
| ccd_setAcqFormat           |      ✅      |        ⛔        |   ⛔    |         |
| ccd_setRoi                 |      ✅      |        ⛔        |   ⛔    |         |
| ccd_getXAxisConversionType |      ✅      |        ✅        |   ⛔    |         |
| ccd_setXAxisConversionType |      ✅      |        ✅        |   ⛔    |         |
| ccd_getDataRetrievalMethod |      ⛔      |        ✖️        |   ✖️    |         |
| ccd_setDataRetrievalMethod |      ⛔      |        ✖️        |   ✖️    |         |
| ccd_getAcqCount            |      ✅      |        ✅        |   ⛔    |         |
| ccd_setAcqCount            |      ✅      |        ✅        |   ⛔    |         |
| ccd_getCleanCount          |      ✅      |        ✅        |   ⛔    |         |
| ccd_setCleanCount          |      ✅      |        ✅        |   ⚠️    |         |
| ccd_getDataSize            |      ✅      |        ⛔        |   ⛔    |         |
| ccd_getTriggerIn           |      ⛔      |        ✖️        |   ✖️    |         |
| ccd_setTriggerIn           |      ⛔      |        ✖️        |   ✖️    |         |
| ccd_getSignalOut           |      ⛔      |        ✖️        |   ✖️    |         |
| ccd_setSignalOut           |      ⛔      |        ✖️        |   ✖️    |         |
| ccd_getAcquisitionReady    |      ✅      |        ✅        |   ⛔    |         |
| ccd_setAcquisitionStart    |      ✅      |        ✅        |   ⛔    |         |
| ccd_getAcquisitionBusy     |      ✅      |        ✅        |   ⛔    |         |
| ccd_setAcquisitionAbort    |      ✅      |        ✖️        |   ✖️    |         |
| ccd_getAcquisitionData     |      ✅      |        ✅        |   ⛔    |         |

# Single Chanel Detector Commands

| Command       | Implemented | Tested | Status | Comment |
| ------------- | :---------: | -----: | -----: | ------: |
| scd_discover  |      ✖️      |      ✖️ |      ✖️ |         |
| scd_list      |      ✖️      |      ✖️ |      ✖️ |         |
| scd_listCount |      ✖️      |      ✖️ |      ✖️ |         |
| scd_open      |      ✖️      |      ✖️ |      ✖️ |         |
| scd_close     |      ✖️      |      ✖️ |      ✖️ |         |
| scd_isOpen    |      ✖️      |      ✖️ |      ✖️ |         |
