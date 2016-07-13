# PiHueSphinx

A simple C# program using pocketsphinx to control Phillips Hue lights using voice commands. I run this on a Raspberry Pi in my home.

aka. my cheap Alexa

## Building

Provided you have installed a recent pocketsphinx library, you should be able to build everything from the IDE or via `xbuild`/`msbuild`.

By recent pocketsphinx library, that would usually mean going to http://cmusphinx.sourceforge.net/wiki/download and building the last version from there as your distribution packages likely haven't caught up.

If the C compilation fails, you can adjust properties and stuff at the end of the `PiHueSphinx.csproj` file to make sure it's finding your copy of libpocketsphinx.

## Usage

`PiHueSphinx.exe CONFIG HOTWORD ALSA_INPUT_DEVICE [HUE_API_KEY]`

Example: `mono --debug PiHueSphinx.exe pocketsphinx.conf "OKAY RASPBERRY" "plug:usb" "<Your Hue API Key>"`

If you want to change the hotword it needs to be part of your corpus and the language model needs to be re-generated, see section below.

The ALSA input device can be whatever ALSA supports, in my case it's a USB camera from which I use the microphone. Refer to ALSA documentation to learn how to setup an input device. Your device might need a different input sampling rate that you can change in the `pocketsphinx.conf` file.

Once the program has associated with your Hue bridge and generated an app key (it will print it on the console), you should start passing it as the last argument.

## Generating pocketsphinx language model

The `corpus.txt` file contains the list of command that the program processes.

The `pisphinx.lm` and `pisphinx.dic` files were generating from that `corpus.txt` file using their online service at http://www.speech.cs.cmu.edu/tools/lmtool-new.html

## Calibrating the program

The `pocketsphinx.conf` file contains the configuration used by pocketsphinx. In it you have the `-kws_threshold` parameter which is a little obscure in every tidbit of documentation that I went through.

The value there depends on your hotword command (in my case "Okay Raspberry") and it's basically by trial and error that you can find a good one. Try to go from 1e-1 to 1e-60.

## Phillips hue API key

When you run the program it will locate the Phillips base station using host discovery. If you haven't passed an API key on the command-line it will try to register itself for one, be sure to press the link big button on top of the station when that happen.

## Libraries used

- Pocketsphinx: https://github.com/cmusphinx/pocketsphinx
- Q42 Hue API: https://github.com/Q42/Q42.HueApi

## License

Code is placed under the Apache 2.0 license
