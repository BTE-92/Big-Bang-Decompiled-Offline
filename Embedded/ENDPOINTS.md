# Routes du client 2.4.9 : état côté serveur

Source : décompilation de `Assembly-CSharp.dll` de l'APK 2.4.9 (`Server/*.cs` + `ServerConfig.cs`).
Le client référence **131 routes** : toutes sont maintenant déclarées par le serveur.
**89 sont réellement implémentées**, les **42 autres** (liste plus bas, `LeftoverApi.cs`) répondent par une réponse vide mais valide
(`{}`, `{"data":[]}` ou `{"count":0}`) : aucun écran du jeu ne les appelle.
Toute route inconnue (hors liste) répond `{}` avec `PLAY_STATUS: OK` et une ligne `[HTTP] not implemented` dans le log.

## Implémentées (89)
Joueur, connexion, amis, niveaux (sauvegarde, publication, recherche, données, captures,
notes, commentaires, récompenses créateur), scores et fantômes, chemin/progression,
défis entre amis, versus, musique (`preload/checkFile`), publicité, événements chronométrés.

## Fonctions visibles en jeu qui dépendaient du serveur
| Fonction | État |
|---|---|
| Défi de diamants du jour (« Daily Gems ») | Fournit un niveau public de mode `StarCollect`, différent chaque jour (UTC). Vide s'il n'existe aucun niveau de ce mode. |
| Fresh and Free | Via `/v1/minigame/oneFresh` : un niveau public au hasard. |
| Versus / défi d'ami | Appariement, tours, vainqueur, fantômes, stats (parties, victoires, défaites). |

## Routes déclarées avec une réponse vide, et pourquoi (42)
Vérifié par analyse des appelants dans le client : aucune n'est atteignable depuis l'interface 2.4.9.

- **Tournois** (`tournament/claim|create|unlock|score/next`, `v2/tournament/score/find|send`) :
  `Tournament.Participate`, `TournamentClaim` ne sont appelés que par `PsMetagameManager.ParticipateTournament` /
  `ClaimTournament`, qui n'ont eux-mêmes aucun appelant. Les listes de tournois ne sont lues nulle part.
- **Modèles de niveau** (`template/*`) : la carte du jeu (`LocalInitialData.txt`) ne contient
  aucun nœud `LevelTemplate`, `EditorPuzzle` ni `VersusRace`. `template/save` est réservé au mode admin (ces routes restent implémentées).
- **Réseaux sociaux** (`player/check|solve|remove|follow/social`, `follow|check|join/gameCenter`, `join/facebook`) :
  nécessitent une connexion Facebook ou Game Center, impossible hors ligne.
- **Achats** (`iap/config`, `iap/purchase`, `analytics/trackPurchase`, `player/tester/*`, `player/rent`) :
  l'IAP n'a aucun appelant ; `trackPurchase` n'est appelé qu'après un vrai achat.
- **Code mort** (aucun appelant) : `ghosts` (v1), `path/get`, `customisation/add`, `player/data/find|setNumber|setString`,
  `player/stats/change|set`, `player/join/ninja`, `push/token/*`, `notification/*`, `planet/current/find`,
  `research/*`, `minigame/level/update`, `minigame/like/save`, `v2/path/db/update` (éditeur interne), `v1/ghost/*` variantes.

## Comment retrouver cette liste
Les scripts d'analyse (extraction des routes du client, comparaison avec le serveur, analyse des appelants)
ne sont pas conservés ; ils se refont en décompilant l'APK avec `ilspycmd` puis en cherchant les chaînes `"/v<N>/..."`.
