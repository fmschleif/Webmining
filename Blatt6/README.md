# Blatt 6: Clustering

## Aufgabe 1: Clustering

Erstellen Sie einen Clustering-Workflow in RapidMiner

- Laden Sie die Iris-Daten
- Normalisieren Sie die Daten (Z-Transformation)
- Nutzen Sie das k-Means Modul
- Visualisieren Sie die original Daten in den Dimensionen 3/4 mit dem Cluster-Label
- variieren Sie die Parameter (z.B. Anzahl der Cluster)
- → Diskussion

## Aufgabe 2: Graph-Clustering

Laden Sie die im elearning System hinterlegten graph clustering daten

- im Web gibt es eine Vielzahl von Graphdaten (z.B. Social networks)
- Implementieren sie den Spectral-Clustering (SC) Algorithmus aus der Vorlesung
- testen Sie den Algorithmus fuer die TwoMoon Data - mit 2 Clustern
- testen Sie den Trinity Datensatz fuer verschiedene k (Anzahl Cluster)
- Visualisieren Sie die Ergebnisse (Tipp: In Matlab z.B. graph, plot)

## Aufgabe 3: (Berliner S/U-Bahn - Netz)

Laden Sie aus dem elearning die Dateien: vbb.csv (Knoten) und vbb edge.csv (Relationen)

- Erstellen Sie eine Graph-Representation zu den Daten
- Clustern Sie die Daten z.B. mit dem SC-Algorithmus (symmetrisiert)
- Visualisieren Sie die Daten und faerben Sie die Knoten anhand der Clusterzuordnung ein
- Nutzen Sie die Zusatzinformationen zu den Daten um eine inhaltliche Beschreibung der Cluster zu erstellen (z.B. Stadtteile) - vergleichen Sie mit einem Ground-Truth