# IMSE Launcher

## Components
- App Launcher (To-Be-Added)
- [Projector controller](#projector-controller)
- [System power controller](#system-power-controller)

## Projector controller
The controller will manipulate the projector by the provided config file with corresponding command set.

This controller has been tested on
- BenQ MW855UST
- ViewSonic LS830
- Barco F50

### Diagram
![diagram of projector controller](Resources/Image/Projector.png)

### Current support
| brand | config | manual |
|-------|--------|--------|
| **BenQ** | [benq.pcfg](Unity_Launcher/Assets/Resources/Project%20Config/benq.pcfg) | [benq_rs232](Resources/ProjectorManual/benq_rs232.pdf) |
| **ViewSonic** | [vs.pcfg](Unity_Launcher/Assets/Resources/Project%20Config/vs.pcfg) | [viewsonic_projector_manual_9599](Resources/ProjectorManual/viewsonic_projector_manual_9599.pdf) |
| **Barco** | [barco.pcfg](Unity_Launcher/Assets/Resources/Project%20Config/barco.pcfg) | [barco-RS232-LAN-UserGuide](Resources/ProjectorManual/barco-RS232-LAN-UserGuide.pdf) |

### Add a new vendor
> for more detailed description of the supported config, please refer to [template config](Unity_Launcher/Assets/Resources/Project%20Config/brand.pcfg.template)

1. Duplicate the template config
2. Check the manual of the projector for the following spec of RS232 / COM PORT,
  - baudrate
  - data length
  - stop bit
  - parity check
3. Input the corresponding spec in the config. Here shows an example,
```
 attr_baudrate       = 119200
 attr_data_length    = 8
 attr_stop_bit       = One
 attr_parity_check   = None
```
4. Check the command string in the manual and replace the corresponding field. Here shows an example,
```
# the command supports both hex or decimal number
cmd_power_on    = 0x06 0x14 0x00 0x04 0x00 0x34 0x11 0x00 0x00 0x5D
cmd_power_off   = 13 42 112 111 119 61 111 102 102 35 13
```
5. Save the config and push to the repository

### Add a new project with saved config in Launcher
1. Launch the IMSE Launcher
2. After entering the app selection scene, press left ALT + right SHIFT + APLHA 2 to trigger the projector menu
3. Click "+"
4. Input port name of the projector connection
5. Input the path of the config file for the projector
6. Leave the last field as default
7. Click "確定" to save the projector settings
8. Restart the launcher and the projector will be in control

## System power controller

### Install the helper software on tracker mahcine
Follow the instructions in this [link](http://gitlab.imse.hku.hk/imse/imse-monitor#pre-built-release) to install the helper software on the tracker machine, so that this launcher do the remote shutdown on the tracker machine.