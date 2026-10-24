# Patchs du client 2.4.9 reportés sur le projet Unity 2018 de Dodo

* `dodo_scripts_before_client_patches\` : les scripts de Dodo avant mes modifications (sauvegarde).
* `client_patches_from_v70.diff` : toutes les modifications de ton client complet `2.4.9_Patched_full_v70.apk` (mode admin, objets dev, éditeur, Import/Export Level, traductions, corrections de plantages) par rapport à l'APK d'origine, SANS `ClientTools.FixDevItem` (le serveur renomme déjà la catégorie « Dev Items » et corrige les icônes).
* `manual_hunks.py` : les morceaux qui ne s'appliquaient pas tels quels sur le code de Dodo, reportés à la main.
* `original_to_Patched_apk.diff` : pour mémoire, ce que contient `APK\2.4.9_Patched.apk` (version « légère » : admin + objets dev + découverte du serveur).

Ajouts qui ne viennent pas des diffs :
* `Assets\Scripts\Assembly-CSharp\BBRClient\` : `Loc.cs` (traductions, table `Assets\Resources\BBR\loc.txt`), `LocUI.cs` (bouton/choix de la langue), `LevelTransfer.cs` (ex-`LevelIO` de ton module : export / import de niveaux en .zip), `WindowsPicker.cs` (fenêtre « Ouvrir » sous Windows).
* Android : `Assets\Plugins\Android\src\com\bbr\picker\PickerActivity.java` + activité dans `AndroidManifest.xml`.
* iOS : `Assets\Editor\PostBuildiOS.cs` (dossier Documents visible dans l'app Fichiers).
* Chapeaux secrets : `AlienCharacter.SetRandomHat` choisit parmi les 9 accessoires du jeu (le jeu d'origine n'utilisait que le casque).
