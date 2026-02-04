-> main

=== main ===
I am a nerd if you want to know!!!! #speaker:0 #portrait:neutral #layout:right
HAHAHAHAHAHAH!!!!

But I want to know... which game was first?
+ [Mario] -> gameChosen("Mario")
+ [Donkey Kong] -> gameChosen("DonkeyKong")
+ [Człowiek Małpa] -> gameChosen("ClowekMalpa")

=== gameChosen(game) ===
{game == "DonkeyKong":
    First game was <b><color=\#FF22F4>Donkey Kong</color></b> #speaker:1 #layout:left
    You are right!!! <link=gradient+wiggle>Congratulations!!!</link> #speaker:0 #portrait:happy #layout:right
    -> END
- else:
    You are wrong!!! #portrait:angry #layout:left
    -> badChoice("Good choice was Donkey Kong")
}

=== badChoice(msg) ===
{msg}

-> END