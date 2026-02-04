EXTERNAL AddGoodAnswer(points)

VAR good = 0

-> main

=== main ===
Um, actually you can't go here. #speaker:0 #portrait:happy #layout:right
Without going through me that is. #portrait:neutral
! #speaker:1 #layout:left
I am an intelectual (a nerd) so I will test your wits and knowledge! #speaker:0 #portrait:happy #layout:right
! #speaker:1 #portrait:neutral #layout:left
-> question_1

=== question_1 ===
So tell me ... which game was first? #speaker:0 #layout:right #portrait:neutral
+ [Zelda]
    Zelda was first. #speaker:1 #layout:left #portrait:neutral
    -> wrong_answer_1
+ [Tetris]
    ~ AddGoodAnswer("1")
    ~ good = good + 1
    Tetris was first. #speaker:1 #layout:left #portrait:neutral
    -> correct_answer_1
+ [Mario]
    Mario was first. #speaker:1 #layout:left #portrait:neutral
    -> wrong_answer_1

=== wrong_answer_1 ===
HAHAHAHAHA!!! You're wrong! #speaker:0 #portrait:happy #layout:right
-> question_2

=== correct_answer_1 ===
It can't be!!! You're correct... #speaker:0 #portrait:surprised #layout:right
-> question_2

=== question_2 ===
But wait, there is more. #speaker:0 #portrait:happy #layout:right
What year did Pokemon anime first air? #portrait:neutral
+ 1997 #speaker:1 #layout:left #portrait:neutral
    ~ AddGoodAnswer("1")
    ~ good = good + 1
    -> correct_answer_2
+ 1995 #speaker:1 #layout:left #portrait:neutral
    -> wrong_answer_2
+ 1993 #speaker:1 #layout:left #portrait:neutral
    -> wrong_answer_2

=== wrong_answer_2 ===
HAHAHAHAHA!!! You're wrong! #speaker:0 #portrait:happy #layout:right
-> question_3

=== correct_answer_2 ===
It can't be!!! You're correct... #speaker:0 #portrait:surprised #layout:right
-> question_3

=== question_3 ===
But now for my final question, the most diffucult one! #speaker:0 #portrait:happy #layout:right
... #portrait:neutral
WHAT CLASS AM I PLAYING IN DUNGEONS AND DRAGONS?! #portrait:happy
+ Rogue #speaker:1 #layout:left #portrait:neutral
    -> wrong_answer_3
+ Cleric #speaker:1 #layout:left #portrait:neutral
    -> wrong_answer_3
+ Wizard #speaker:1 #layout:left #portrait:neutral
    ~ AddGoodAnswer("1")
    ~ good = good + 1
    -> correct_answer_3

=== wrong_answer_3 ===
HAHAHAHAHA!!! You're wrong! #speaker:0 #portrait:happy #layout:right
-> end_answer

=== correct_answer_3 ===
It can't be!!! You're correct... #speaker:0 #portrait:surprised #layout:right
-> end_answer


=== end_answer ===
{good == 3:
    <b>GASP</b> <link=wiggle>IMPOSSIBLE!!!</link> You're correct... #speaker:0 #portrait:surprised #layout:right
    ... #portrait:neutral
    You've bested me. You've gained the respect of my club of intelectuals (nerds)!
    Now I allow you to pass senpai.
- else:
    You activated my trap card, and now you shall face my <link=w3>wrath!!!</link> #speaker:0 #portrait:happy #layout:right
}
-> END