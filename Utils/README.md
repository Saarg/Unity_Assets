# MultiOSControls
MultiOSControls is a script made to be used instead of the default input manager.
It let you setup different controls under a name and asign controler axis,
controller buttons and keyboard inputs without thinking about the OS.
for now it only uses one controller but will do more than one in the futur.
## Setup
To setup MultiOSControls just add the script to an empty GameOject which we will
call Scripts from now.

Now you can add inputs in the editor, every inputs can have many positive keys,
negative keys, buttons, and controller axis. You can also set a deadzone for the
axis to ensure return position is 0.
## Use
To use the inputs in a script just define:
```
private MultiOSControls _controls;
...
_controls = GameObject.Find ("Scripts").GetComponent<MultiOSControls> ();
```

Then just use `_controls.getValue ("youControl")` wich will return between -1 to
1 with a default value of 0.
