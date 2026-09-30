# ConduiteFacile.Demo — solution du challenge de fin de Module 2

Solution complète correspondant à l'énoncé `Challenge_Fin_Module2_ConduiteFacile.docx`
(noms français). Traite les Parties A à D et le bonus, dans l'ordre de l'énoncé.

## Mise en place

```bash
cd ConduiteFacile.Demo
dotnet restore
dotnet ef migrations add InitConduiteFacile
dotnet ef database update
dotnet run
```

Comme pour les autres livrables de ce parcours, les migrations ne sont pas
fournies comme fichiers : `Migrations/*.cs` est généré par `dotnet ef`, pas
écrit à la main. La commande ci-dessus régénère l'unique migration nécessaire
(le modèle ne change plus après l'insertion des données, contrairement à la
démonstration de la Journée 2).

## Ce que fait `dotnet run`

1. Applique la migration, puis insère le jeu de données de l'énoncé si la base est vide.
2. Partie B : ajoute Maxime Petit, passe une leçon à Realisee (sans appel explicite à `Update`), supprime la leçon annulée, puis provoque et intercepte une violation de clé étrangère.
3. Partie C : exécute les 6 requêtes de contrôle et affiche leurs résultats.
4. Partie D : affiche la version corrigée des deux extraits fautifs de l'énoncé (les versions fautives figurent en commentaire, juste au-dessus, pour référence).
5. Bonus : ajoute une leçon et incrémente un compteur sur le moniteur dans une transaction explicite.

## Non vérifié par exécution

Cet environnement ne dispose pas du SDK .NET (ni d'accès réseau pour restaurer
les packages) : ce code n'a donc pas pu être compilé ni exécuté ici. Les
résultats attendus (C.1 à C.6) ont été vérifiés par un calcul indépendant sur
le même jeu de données lors de la préparation de l'énoncé — mais une
vérification par `dotnet run` reste recommandée avant diffusion comme corrigé.
