# Blatt 4: Data Encoding II + Eigenvalues

## [Aufgabe 1: Eigenwert-Analyse](Aufgabe1.m)

Laden Sie die Daten `iris_test.mat` aus dem elearning-System in Matlab.
- Normalisieren Sie die Daten bzgl Mittelwert
- Berechnen Sie eine Kovarianzmatrix C der Daten
- Schreiben Sie eine Funktion die das von Mises Verfahren auf einer Matrix realisiert
- Wenden Sie die Funktion auf der Kovarianzmatrix C an und berechnen Sie den größten Eigenvektor
- Berechnen Sie nun den dazugehörigen Eigenwert
- Berechnen Sie die Eigenwerte / Eigenvektoren der Matrix C mit MatlabKommandos - Vergleichen Sie 
- Plotten Sie ein Eigenspektrum zu C

## [Aufgabe 2: Bag of Words](Aufgabe2.m)

Laden Sie die Datei bow `newsletter.data` herunter
- Verwenden Sie die Funktionen in `Convert2FullMatrix.m` um aus den Daten eine quadratische Matrix zu erzeugen
- Die Daten sind hier fur 2000 newsletter posts mit 20 Gruppen gegeben
- Visualisieren Sie die Matrix
- → Diskussion

## Aufgabe 3: Nystroem

Versuchen Sie die BoW Matrix aus Aufgabe 2 per Nystroem zu approximieren. Nutzen Sie dazu unterschiedlich viele Landmarks. Bewerten Sie die Rekonstruktionsguete mit der Frobenius norm.
