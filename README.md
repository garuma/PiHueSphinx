# PiHueSphinx

A simple C# program using pocketsphinx to control Phillips Hue lights using voice commands.

aka. my cheap Alexa

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
