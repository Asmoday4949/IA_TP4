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

## Heuristique

Pour l'heuristique, nous utilisons les x valeurs suivantes pour déterminer le meilleur coup à jouer :

- "Coin parity" : différence de pions entre le "max player" et le "min player"
- Mobilité : nombre de coups possibles pour le "max player" et le "min player", dans le but d'avoir une grande mobilité pour soit et une petite mobilité pour l'adversaire
- Nombre de pièces dans les coins : ces pions ont une importance, car ils ne peuvent pas être retrournés par l'adversaire

... (a compléter ?)

## Sources

Heuristique : https://kartikkukreja.wordpress.com/2013/03/30/heuristic-function-for-reversiothello/
