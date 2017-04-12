# Blatt 1: Wiederholung Matlab

## [Aufgabe1: Befehle und Operationen](Aufgabe1.m)

Starten Sie Matlab und erzeugen Sie folgende Variablen:
- einen Zeilenvektor v<sub>1</sub> der Dimension 7 mit Einträgen 2 (alle)
- einen Spaltenvektor v<sub>2</sub> der Dimension 7 mit Einträgen 1, ... ,7
- eine 7x7-Matrix M mit Einträgen 1, ... ,7<sup>2</sup> zeilenweise durchgezählt

Lassen Sie von der Matrix M folgende Teile ausgeben:
- das Element in der 3. Zeile und 6. Spalte
- die gesamte 3. Zeile
- die gesamte 6. Spalte
- die 3x2-Untermatrix aus den Einträgen der 4., 5. & 6. Spalte und 1. & 2. Zeile
- die Summe der Diagonale
- die obere Dreiecksmatrix
- alle Einträge, die größer als 33 sind
- alle Einträge, die ungerade sind

Welche der folgenden Operationen sind in Matlab gültig und was tun sie?
- v<sub>1</sub> · v<sub>2</sub>
- v'<sub>1</sub> · v<sub>2</sub>
- $M · v'<sub>1</sub>$
- M(3,:) + v<sub>1</sub>
- M == M'