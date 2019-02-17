# IA - Othello/Reversi

Dans le cadre du cours d'IA, il a été demandé de développer un IA pour le jeu de reversi. L'algorithme à mettre en place est appelé "MinMax" ou "AlphaBeta".
Cette IA doit être contenue dans une bibliothèque C# et elle doit respecter l'interface IPlayable.

## Interface IPlayable

```
string GetName();
bool IsPlayable(int column, int line, bool isWhite);
bool PlayMove(int column, int line, bool isWhite);
Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn);
int[,] GetBoard();
int GetWhiteScore();
int GetBlackScore();
```

## Algorithme

Pour ce travail, nous nous sommes basés sur l'algorithme "AlphaBeta" mis à diposition dans le cours.
Celui-ci a été légèrement modifier afin de correspondre au jeu d'Othello.

## Heuristique

Pour l'heuristique, nous utilisons les valeurs suivantes pour déterminer le meilleur coup à jouer :

- "Coin parity" : différence de pions entre le "max player" et le "min player".
- Mobilité : nombre de coups possibles pour le "max player" et le "min player", dans le but d'avoir une grande mobilité pour soit et une petite mobilité pour l'adversaire.
- Nombre de pièces dans les coins : ces pions ont une importance, car ils ne peuvent pas être retournés par l'adversaire.
- "Disc Square" : utilisation d'un tableau contenant une valeur pour chaque case du jeu. Le principe est de mettre des scores élévés aux cases qui sont des coups intéressants.
- Stabilité : vérifie qu'un pion soit stable (pas de risque d'être pris au prochain tour)

## Release

Il est possible de trouver une DLL précompilée ici : https://www.dropbox.com/s/nq49hp59xpxtoj6/OthelloIAFH.zip?dl=0

## Utilisation

Mettre à la racine du programme "Tournament" la bibliothèque et effectuer une recherche d'IA dans le menu "Options" du programme "Tournament".

## Sources

Heuristique :
https://kartikkukreja.wordpress.com/2013/03/30/heuristic-function-for-reversiothello/
https://en.wikipedia.org/wiki/Computer_Othello
https://stackoverflow.com/questions/12334216/othello-evaluation-function/12334779#12334779
