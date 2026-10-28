# Spécification de travail — Calcul des courants admissibles
## NF C 13-200, partie 5-52

**Version :** première version consolidée à partir des captures fournies  
**Statut :** spécification de travail — transcription à vérifier avant intégration définitive  
**Langue :** français

---

## 0. Consignes essentielles à l’agent

Ce document rassemble les informations lisibles dans les captures fournies et les règles de calcul attendues. L’agent doit pouvoir poursuivre le travail sans avoir accès au PDF initial.

### Ne pas inventer

- Ne pas compléter les valeurs absentes.
- Ne pas traiter silencieusement un facteur de correction nécessaire mais non documenté comme égal à 1.
- Ne pas interpoler ou extrapoler les facteurs ou les courants des tableaux sans règle explicitement validée.
- Ne pas extrapoler le courant admissible au-delà des sections disponibles.
- Si une image, une cellule ou une règle est ambiguë, la marquer **« à vérifier »** et demander une capture complémentaire.
- Séparer les informations en trois catégories :
  - **Confirmé par capture** : valeur ou texte lisible dans une capture fournie ;
  - **Règle de calcul / interprétation** : comportement logiciel déduit de la spécification ;
  - **À vérifier / non documenté** : information absente, ambiguë ou nécessitant validation.

### Traçabilité

Pour chaque donnée intégrée, conserver autant que possible :

- le numéro du tableau ou de l’article ;
- l’édition de la norme, lorsqu’elle sera connue ;
- la page de la capture, lorsqu’elle est visible ;
- l’unité ;
- les conditions d’application ;
- les notes associées ;
- le statut de vérification de la transcription.

Les captures fournies sont la source de transcription de cette version. Elles ne constituent pas, à elles seules, une validation indépendante de conformité normative.

---

## 1. Objectif du logiciel

Créer un logiciel d’aide au calcul proposant les fonctions suivantes :

1. Calculer le facteur global de correction, c’est-à-dire le produit des facteurs applicables et documentés.
2. Calculer le courant admissible corrigé pour une section et une configuration de câble données.
3. Déterminer la plus petite section normalisée disponible dont le courant admissible corrigé est supérieur ou égal au courant demandé.

Le logiciel doit expliquer son résultat, notamment :

- tableau de courant de référence utilisé ;
- courant de référence et unité ;
- facteurs appliqués, valeurs et justification ;
- facteur global ;
- courant admissible corrigé ;
- hypothèses et conditions de validité ;
- facteurs potentiellement nécessaires mais manquants ;
- avertissements et limites du calcul.

Le calcul thermique ne constitue pas, à lui seul, une validation complète d’un dimensionnement électrique.

---

## 2. Portée et limites

La partie 5-52 traite des courants admissibles des câbles en service normal, en régime permanent, sous des conditions de pose définies.

Les tableaux de référence donnent les courants admissibles sous des conditions précises. Lorsque les conditions réelles diffèrent, un ou plusieurs facteurs de correction peuvent être nécessaires.

Le logiciel est une **aide au calcul dans le périmètre documenté**. Il ne doit pas se présenter comme une certification complète de conformité ou comme un substitut à toutes les vérifications de l’installation, notamment :

- chute de tension ;
- protection contre les effets thermiques ;
- protection contre les courts-circuits ;
- coordination avec les dispositifs de protection ;
- autres exigences réglementaires ou normatives applicables.

---

## 3. Formules générales

### 3.1 Facteur global

Pour les facteurs applicables et documentés :

```text
facteur_global = produit de tous les facteurs applicables
```

Exemple :

```text
facteur_global = K1 × K3 × K4 × ... × fs
```

Tous les facteurs ne s’appliquent pas à chaque installation. Leur sélection dépend du mode de pose et des conditions réelles.

Un facteur manquant mais potentiellement nécessaire doit produire un **calcul partiel** ou un **calcul impossible**. Il ne doit jamais être remplacé silencieusement par 1.

### 3.2 Courant admissible corrigé

```text
courant_corrigé = courant_de_référence × facteur_global
```

Le courant de référence doit être sélectionné dans le tableau adapté :

- au type de câble ;
- à l’isolant ;
- au matériau du conducteur ;
- à la section ;
- au mode de pose ;
- et, lorsqu’il y a lieu, à la tension assignée et à la disposition des câbles.

### 3.3 Recherche de la section minimale

Pour le courant demandé `I_demande`, sélectionner la plus petite section disponible `S` telle que :

```text
courant_corrigé(S) >= I_demande
```

Règles :

- ne rechercher que parmi les sections explicitement disponibles dans le tableau retenu ;
- ne pas interpoler entre sections ;
- ne pas extrapoler au-delà de la plus grande section ;
- signaler si aucune section tabulée ne satisfait le courant demandé.

---

## 4. États de calcul à implémenter

Le logiciel doit au minimum distinguer les états suivants.

### 4.1 Calcul complet dans le périmètre documenté

Le tableau de référence est sélectionné et tous les facteurs requis par les entrées fournies sont documentés et tabulés.

Cet état signifie seulement que le calcul est complet **au regard des données intégrées au logiciel**. Il ne signifie pas que toute la conception électrique est validée.

### 4.2 Calcul partiel

Un calcul numérique peut être effectué pour les facteurs documentés, mais un facteur potentiellement applicable manque ou la configuration dépasse les cas entièrement documentés.

Exemples :

- K2 nécessaire parce que le câble est exposé directement au soleil, mais valeur de K2 absente ;
- disposition de câbles parallèles ne correspondant pas aux exemples documentés de `fs` ;
- température ou espacement non couvert par les données tabulées et sans formule validée.

Le résultat partiel doit l’indiquer clairement et ne pas afficher un courant corrigé comme résultat définitif sans avertissement visible.

### 4.3 Calcul impossible

Aucune sélection fiable du tableau de référence ou des facteurs nécessaires n’est possible avec les entrées fournies.

Exemples :

- type de câble incompatible ou insuffisamment renseigné ;
- tension assignée nécessaire mais absente ;
- configuration non documentée ;
- donnée obligatoire manquante.

---

## 5. Tableaux de courants admissibles de référence

Les valeurs numériques ci-dessous sont transcrites depuis les captures reçues. Elles doivent être vérifiées cellule par cellule avant leur intégration dans une version destinée à un usage réel.

Dans chaque tableau, les colonnes de courant sont présentées dans l’ordre :

```text
Cuivre à l’air | Cuivre enterré | Aluminium à l’air | Aluminium enterré
```

Unité des courants : **ampères (A)**.  
Unité des sections : **mm²**.

### 5.1 Tableau 52G

**Description relevée :**

- câbles tripolaires à champ radial ;
- tension assignée : 3,6/6 (7,2) kV à 18/30 (36) kV ;
- isolant PR ou EPR ;
- conducteur cuivre ou aluminium ;
- pose à l’air ou enterrée ;
- température maximale de l’âme en régime permanent : 90 °C ;
- sections visibles : 16 à 300 mm².

| Section (mm²) | Cuivre à l’air (A) | Cuivre enterré (A) | Aluminium à l’air (A) | Aluminium enterré (A) |
|---:|---:|---:|---:|---:|
| 16 | 120 | 126 | 94 | 98 |
| 25 | 156 | 160 | 121 | 125 |
| 35 | 189 | 192 | 146 | 148 |
| 50 | 227 | 226 | 175 | 175 |
| 70 | 282 | 277 | 220 | 215 |
| 95 | 345 | 331 | 268 | 257 |
| 120 | 400 | 377 | 309 | 292 |
| 150 | 456 | 423 | 351 | 327 |
| 185 | 523 | 478 | 407 | 372 |
| 240 | 619 | 554 | 479 | 431 |
| 300 | 718 | 627 | 552 | 487 |

### 5.2 Tableau 52H

**Description relevée :**

- câbles tripolaires à champ non radial ;
- tension assignée égale à 6/6 (7,2) kV ;
- isolant PR ou EPR ;
- conducteur cuivre ou aluminium ;
- pose à l’air ou enterrée ;
- température maximale de l’âme en régime permanent : 90 °C ;
- sections visibles : 10 à 240 mm².

| Section (mm²) | Cuivre à l’air (A) | Cuivre enterré (A) | Aluminium à l’air (A) | Aluminium enterré (A) |
|---:|---:|---:|---:|---:|
| 10 | 78 | 86 | 60 | 67 |
| 16 | 100 | 110 | 79 | 86 |
| 25 | 130 | 145 | 105 | 110 |
| 35 | 165 | 170 | 125 | 135 |
| 50 | 205 | 215 | 160 | 165 |
| 70 | 255 | 260 | 195 | 205 |
| 95 | 310 | 315 | 240 | 245 |
| 120 | 360 | 360 | 280 | 280 |
| 150 | 410 | 405 | 320 | 315 |
| 185 | 460 | 450 | 360 | 350 |
| 240 | 550 | 525 | 430 | 410 |

### 5.3 Tableau 52I

**Description relevée :**

- câbles tripolaires à champ non radial ;
- tension assignée égale à 6/6 (7,2) kV ;
- isolant PVC ;
- conducteur cuivre ou aluminium ;
- pose à l’air ou enterrée ;
- température maximale de l’âme en régime permanent : 70 °C ;
- sections visibles : 10 à 240 mm².

| Section (mm²) | Cuivre à l’air (A) | Cuivre enterré (A) | Aluminium à l’air (A) | Aluminium enterré (A) |
|---:|---:|---:|---:|---:|
| 10 | 62 | 70 | 48 | 56 |
| 16 | 81 | 94 | 62 | 72 |
| 25 | 105 | 120 | 82 | 94 |
| 35 | 130 | 145 | 100 | 115 |
| 50 | 165 | 185 | 130 | 145 |
| 70 | 205 | 225 | 160 | 175 |
| 95 | 250 | 270 | 195 | 210 |
| 120 | 290 | 310 | 225 | 240 |
| 150 | 330 | 345 | 255 | 270 |
| 185 | 370 | 385 | 285 | 300 |
| 240 | 440 | 445 | 345 | 350 |

**Condition enterrée telle que libellée dans la capture de 52I :**

- un seul circuit sans proximité thermique et électrique ;
- « température ambiante maximum : 20 °C » ;
- résistivité thermique du sol : 1 K·m/W ;
- profondeur de pose : 800 mm.

Le libellé « température ambiante maximum : 20 °C » est conservé fidèlement à la capture et doit être marqué **à vérifier** avant toute interprétation ou modification.

### 5.4 Tableau 52J

**Description relevée :**

- circuits constitués de trois câbles unipolaires à champ radial ;
- tension assignée : 3,6/6 (7,2) kV à 18/30 (36) kV ;
- isolant PR ou EPR ;
- conducteur cuivre ou aluminium ;
- pose à l’air ou enterrée ;
- configurations en trèfle ou en nappe ;
- température maximale de l’âme en régime permanent : 90 °C ;
- sections visibles : 16 à 1 200 mm².

La capture précise :

- les valeurs ont été calculées avec l’écran relié à la terre aux deux extrémités ;
- elles sont également valables pour un écran relié à une seule extrémité ;
- la pose en trèfle est recommandée pour un meilleur équilibrage des phases.

Colonnes, dans l’ordre :

```text
Cuivre à l’air trèfle |
Cuivre à l’air nappe |
Cuivre enterré trèfle |
Cuivre enterré nappe |
Aluminium à l’air trèfle |
Aluminium à l’air nappe |
Aluminium enterré trèfle |
Aluminium enterré nappe
```

| Section (mm²) | Cu air trèfle | Cu air nappe | Cu enterré trèfle | Cu enterré nappe | Al air trèfle | Al air nappe | Al enterré trèfle | Al enterré nappe |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 16 | 120 | 123 | 126 | 129 | 94 | 96 | 98 | 101 |
| 25 | 157 | 161 | 161 | 165 | 122 | 125 | 125 | 128 |
| 35 | 190 | 195 | 192 | 197 | 147 | 151 | 149 | 153 |
| 50 | 233 | 244 | 225 | 231 | 185 | 189 | 175 | 179 |
| 70 | 292 | 304 | 276 | 283 | 226 | 236 | 214 | 220 |
| 95 | 356 | 369 | 330 | 338 | 266 | 285 | 252 | 262 |
| 120 | 409 | 423 | 375 | 383 | 318 | 330 | 291 | 299 |
| 150 | 465 | 478 | 420 | 430 | 360 | 370 | 325 | 334 |
| 185 | 533 | 549 | 474 | 484 | 417 | 430 | 370 | 379 |
| 240 | 630 | 646 | 549 | 559 | 490 | 504 | 428 | 439 |
| 300 | 724 | 735 | 619 | 623 | 567 | 579 | 485 | 492 |
| 400 | 836 | 838 | 698 | 703 | 662 | 669 | 554 | 562 |
| 500 | 959 | 958 | 786 | 785 | 771 | 776 | 631 | 637 |
| 630 | 1 108 | 1 108 | 887 | 886 | 897 | 905 | 720 | 727 |
| 800 | 1 255 | 1 244 | 980 | 970 | 1 037 | 1 040 | 810 | 812 |
| 1 000 | 1 390 | 1 366 | 1 063 | 1 042 | 1 165 | 1 160 | 895 | 890 |
| 1 200 | 1 480 | 1 445 | 1 117 | 1 087 | 1 264 | 1 252 | 957 | 945 |

### 5.5 Conditions de référence des tableaux 52G à 52J

Conditions indiquées pour la pose à l’air :

- un seul circuit ;
- aucune proximité thermique ou électrique ;
- température ambiante maximale : 30 °C ;
- à l’abri des rayons solaires.

Conditions indiquées pour la pose enterrée dans les captures :

- un seul circuit ;
- aucune proximité thermique ou électrique ;
- température du sol : 20 °C ;
- résistivité thermique du sol : 1 K·m/W ;
- profondeur de pose : 0,80 m.

La capture de 52I emploie « température ambiante maximum : 20 °C » pour le cas enterré. Conserver ce libellé comme **à vérifier** et ne pas le normaliser silencieusement.

---

## 6. Facteurs de correction documentés

### 6.1 K1 — Température ambiante de l’air

**Application :** câbles posés à l’air libre.

| Température de l’air (°C) | 10 | 15 | 20 | 25 | 30 | 35 | 40 | 45 | 50 | 55 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| PVC | 1,22 | 1,17 | 1,12 | 1,06 | 1,00 | 0,94 | 0,87 | 0,79 | 0,71 | 0,61 |
| PR/EPR | 1,15 | 1,12 | 1,08 | 1,04 | 1,00 | 0,96 | 0,91 | 0,87 | 0,82 | 0,76 |

Formule affichée dans la capture pour d’autres températures :

```text
K1 = sqrt((theta2 - theta1) / (theta2 - 30))
```

avec :

- `theta1` : température ambiante de l’air, en °C ;
- `theta2` : température maximale admissible de l’âme :
  - 70 °C pour PVC ;
  - 90 °C pour PR/EPR.

La formule est confirmée comme lisible dans la capture. Son domaine d’application, ses limites et la règle d’arrondi restent non documentés.

**Règle logicielle prudente :**

- utiliser une valeur tabulée pour une température tabulée ;
- appliquer la formule à une autre température seulement si le responsable produit valide son usage pour le domaine considéré ;
- sinon signaler une température hors tableau et ne pas produire de résultat complet.

### 6.2 K2 — Exposition directe au soleil

Le tableau 52K identifie K2 comme le facteur pour l’exposition directe au soleil et renvoie à l’IEC 60287.

Aucune valeur ni formule complète n’est disponible dans les captures fournies.

**Règle logicielle :** si K2 est nécessaire, ne pas utiliser de valeur par défaut. Marquer le calcul comme partiel ou impossible.

### 6.3 K3 — Simple couche sur murs, planchers ou tablettes non perforées

Pour les câbles jointifs :

| Nombre de câbles ou groupes | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| K3 | 1,00 | 0,85 | 0,79 | 0,75 | 0,73 | 0,72 | 0,72 | 0,71 | 0,70 |

La valeur de cinq câbles, 0,73, est confirmée sur la capture nette.

Pour les câbles non jointifs :

```text
si e >= 2d : K3 = 1
```

avec :

- `e` : écartement entre les câbles, bord à bord ;
- `d` : diamètre du plus gros câble.

Le schéma de la capture illustre la disposition et doit être conservé si des schémas explicatifs sont intégrés à l’application.

### 6.4 K4 — Simple couche au plafond

Pour les câbles jointifs :

| Nombre de câbles ou feeders | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| K4 | 0,95 | 0,85 | 0,76 | 0,72 | 0,69 | 0,67 | 0,66 | 0,65 | 0,64 |

Pour les câbles non jointifs :

```text
si e >= 2d : K4 = 0,95
```

Définitions de `e` et `d` : voir K3.

Ne pas appliquer K4 à d’autres modes de pose sans justification normative.

### 6.5 K5 — Simple couche sur tablettes perforées

Pour les câbles jointifs :

| Nombre de câbles ou groupes | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| K5 | 1,00 | 0,88 | 0,82 | 0,77 | 0,75 | 0,73 | 0,73 | 0,72 | 0,72 |

La capture montre une condition de câbles non jointifs avec `h >= 30 cm` et indique :

```text
si e >= 2d : K5 = 1
```

Définitions :

- `e` : écartement entre câbles, bord à bord ;
- `d` : diamètre du plus gros câble ;
- `h` : écartement vertical entre deux câbles.

Conserver les conditions exactes montrées dans le schéma. Ne pas généraliser K5 à une autre disposition de pose.

### 6.6 K6 — Simple couche sur échelles à câbles, corbeaux ou treillis soudés

Pour les câbles jointifs :

| Nombre de câbles ou groupes | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| K6 | 1,00 | 0,88 | 0,82 | 0,80 | 0,80 | 0,79 | 0,79 | 0,78 | 0,78 |

La capture montre une condition de câbles non jointifs avec `h >= 30 cm` et indique :

```text
si e >= 2d : K6 = 1
```

Définitions de `e`, `d` et `h` : voir K5.

### 6.7 K7 — Pose en plusieurs couches

| Nombre de couches | 1 | 2 | 3 | 4 |
|---|---:|---:|---:|---:|
| K7 | 1,00 | 0,80 | 0,73 | 0,70 |

La capture précise que K7 est à multiplier par l’un des facteurs K3, K4, K5 ou K6 selon le support et la configuration.

Exemple visible dans la capture :

- trois couches ;
- cinq câbles jointifs par couche ;
- chemin de câbles perforé ;
- K7 = 0,73 ;
- K5 = 0,75 ;
- facteur résultant affiché : `0,73 × 0,75 = 0,55`.

La capture indique une limite de 36 câbles au total et précise :

- au plus 9 câbles par couche ;
- au plus 4 couches.

La valeur exacte non arrondie du produit est 0,5475. La capture affiche 0,55. La règle générale d’arrondi n’est pas documentée.

**Règle logicielle :** conserver la précision des facteurs sources dans le calcul et préciser toute règle d’arrondi d’affichage. Ne pas considérer l’arrondi de l’exemple comme une règle générale confirmée.

### 6.8 K8 — Câbles posés en conduits jointifs à l’air libre

La capture indique : **un câble, une terne ou une torsade par conduit**.

Lignes : nombre de couches.  
Colonnes : nombre de conduits par couche.

| Couches \ Conduits par couche | 1 | 2 | 3 | 4 | 5 | 6 |
|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0,96 | 0,94 | 0,91 | 0,88 | 0,87 | 0,86 |
| 2 | 0,92 | 0,87 | 0,84 | 0,81 | 0,80 | 0,79 |
| 3 | 0,85 | 0,81 | 0,78 | 0,76 | 0,75 | 0,74 |
| 4 | 0,82 | 0,78 | 0,74 | 0,73 | 0,72 | 0,72 |
| 5 | 0,80 | 0,76 | 0,72 | 0,71 | 0,70 | 0,70 |
| 6 | 0,79 | 0,75 | 0,71 | 0,70 | 0,69 | 0,68 |

### 6.9 K9 — Câbles enfermés

Le titre précise : câbles enfermés, notamment goulottes apparentes ou câbles encastrés dans des planchers.

| Nombre de câbles ou groupes | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 12 | 16 | 20 |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| K9 | 0,80 | 0,70 | 0,65 | 0,60 | 0,55 | 0,55 | 0,50 | 0,50 | 0,45 | 0,40 | 0,40 | 0,40 |

Les schémas montrent notamment :

- câbles mono- ou multiconducteurs dans des conduits encastrés dans une paroi alvéolée ou monolithique de béton ;
- câbles mono- ou multiconducteurs dans des goulottes fermées ;
- câble dans un conduit en montage apparent vertical.

Aucune règle n’est documentée pour les nombres non affichés. Ne pas interpoler ou extrapoler sans validation.

### 6.10 K10 — Caniveaux fermés, non remplis et affleurant le sol

La capture définit le coefficient d’occupation :

```text
coefficient d’occupation = p / somme(e_n)
```

avec :

- `p = 2a + b` : partie du périmètre du caniveau participant à la dissipation thermique vers le milieu environnant, hors partie à l’air libre, c’est-à-dire le couvercle ;
- `somme(e_n)` : somme des encombrements des différents circuits présents dans le caniveau.

Le texte de la capture indique :

- câble tripolaire : encombrement égal au diamètre extérieur du câble ;
- terne de câbles unipolaires : texte ambigu sur le nombre de fois le diamètre ;
- le schéma distingue une pose en trèfle et une pose en nappe ;
- la transcription disponible mentionne deux fois « nappe jointe » dans les puces décrivant les ternes, alors que le schéma distingue trèfle et nappe.

**Point à vérifier :** ne pas implémenter la règle d’encombrement des câbles unipolaires sans capture plus nette ou validation humaine. Conserver l’ambiguïté explicitement.

| Coefficient d’occupation | 5 | 7 | 10 | 15 | 20 | 30 | 50 |
|---:|---:|---:|---:|---:|---:|---:|---:|
| K10 | 0,60 | 0,66 | 0,72 | 0,80 | 0,85 | 0,90 | 0,92 |

La capture recommande de répartir les câbles sur toute la section utile du caniveau.

La méthode de sélection pour une valeur de coefficient d’occupation entre deux colonnes n’est pas documentée. Ne pas interpoler sans validation.

### 6.11 K11 — Câbles souples pour alimentation d’engins mobiles

#### Tambour monopire

| Nombre de spires | 2 | 3 | 4 | 5 | 6 | 8 | 10 | 12 | 14 |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| K11 | 0,88 | 0,80 | 0,75 | 0,74 | 0,73 | 0,72 | 0,70 | 0,69 | 0,65 |

#### Tambour multispire

| Nombre de couches complètes | 1 | 2 | 3 | >3 |
|---:|---:|---:|---:|---:|
| K11 | 0,80 | 0,60 | 0,50 | 0,35 |

Ne pas extrapoler pour un nombre de spires non tabulé sans règle validée.

### 6.12 K12 — Température du sol

| Température du sol (°C) | 0 | 5 | 10 | 15 | 20 | 25 | 30 | 35 | 40 | 45 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| PVC | 1,18 | 1,14 | 1,10 | 1,05 | 1,00 | 0,95 | 0,89 | 0,84 | 0,77 | 0,71 |
| PR/EPR | 1,13 | 1,10 | 1,07 | 1,04 | 1,00 | 0,96 | 0,93 | 0,89 | 0,85 | 0,80 |

Formule affichée pour d’autres températures :

```text
K12 = sqrt((theta2 - theta1) / (theta2 - 20))
```

avec :

- `theta1` : température du sol ;
- `theta2` : température maximale admissible de l’âme :
  - 70 °C pour PVC ;
  - 90 °C pour PR/EPR.

Domaine d’application, limites et arrondi de la formule : non documentés.

### 6.13 K13 — Résistivité thermique du sol

| Résistivité (K·m/W) | Humidité | Nature du terrain indiquée dans le tableau | Ensemble de trois câbles unipolaires | Câbles tripolaires |
|---:|---|---|---:|---:|
| 0,50 | Terrain très humide | sable | 1,25 | 1,20 |
| 0,70 | Terrain humide | sable | 1,14 | 1,10 |
| 0,85 | Terrain dit normal | zone de transition sable / argile et calcaire | 1,06 | 1,05 |
| 1,00 | Terrain sec | argile et calcaire | 1,00 | 1,00 |
| 1,20 | Terrain très sec | zone de transition argile et calcaire / cendres et mâchefer | 0,93 | 0,95 |
| 1,50 | Terrain très sec | cendres et mâchefer | 0,85 | 0,88 |
| 2,00 | Terrain très sec | cendres et mâchefer | 0,75 | 0,79 |
| 2,50 | Terrain très sec | cendres et mâchefer | 0,68 | 0,72 |
| 3,00 | Terrain très sec | cendres et mâchefer | 0,62 | 0,68 |

Les associations exactes des zones de transition doivent être vérifiées visuellement si le logiciel doit choisir K13 en fonction de la seule nature du terrain.

Notes :

- pour les caniveaux câblés, voir 523.3.2 ;
- le coefficient est choisi en fonction des conditions les plus défavorables sur la longueur totale de la liaison ;
- sans prescription particulière, la résistivité thermique du béton est prise à 1 K·m/W.

Privilégier une résistivité thermique explicitement fournie, si elle est disponible, plutôt qu’une déduction automatique à partir du nom du terrain.

### 6.14 K14 — Profondeur de pose

| Profondeur (m) | 0,50 | 0,60 | 0,80 | 1,00 | 1,25 | 1,50 | 1,75 | 2,00 |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| K14 | 1,04 | 1,02 | 1,00 | 0,97 | 0,95 | 0,93 | 0,91 | 0,90 |

Ne pas interpoler ou extrapoler sans règle validée.

### 6.15 K15 — Nombre de circuits enterrés

La distance `a` est celle illustrée entre câbles multiconducteurs ou entre groupements de trois câbles unipolaires.

| Nombre de câbles ou de circuits | Nulle, câbles jointifs | 0,25 m | 0,5 m | 1,0 m |
|---:|---:|---:|---:|---:|
| 2 | 0,76 | 0,84 | 0,88 | 0,92 |
| 3 | 0,64 | 0,74 | 0,79 | 0,85 |
| 4 | 0,57 | 0,69 | 0,75 | 0,82 |
| 5 | 0,52 | 0,65 | 0,71 | 0,80 |
| 6 | 0,48 | 0,60 | 0,69 | 0,78 |
| 7 | 0,44 | 0,58 | 0,68 | 0,76 |
| 8 | 0,41 | 0,57 | 0,67 | non tabulé |
| 9 | 0,39 | 0,55 | 0,66 | non tabulé |
| 10 | 0,37 | 0,54 | non tabulé | non tabulé |
| 11 | 0,36 | 0,53 | non tabulé | non tabulé |
| 12 | 0,35 | 0,52 | non tabulé | non tabulé |

Note de la capture :

> Lorsqu’un circuit est constitué de plusieurs câbles par phase, il y a lieu de prendre en compte autant de circuits que de câbles par phase.

Pour plusieurs couches, la note renvoie à l’IEC 60287-1-1. Ne pas appliquer une valeur non tabulée par interpolation.

### 6.16 K16 — Câbles en conduits jointifs enterrés ou noyés dans le béton

Conditions affichées :

- un câble par conduit ;
- résistivité thermique du béton : 1 K·m/W.

Lignes : nombre de nappes jointives et superposées.  
Colonnes : nombre de tubes jointifs par nappe.

| Nappes \ Tubes par nappe | 1 | 2 | 3 | 4 | 5 | 6 |
|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0,80 | 0,70 | 0,62 | 0,58 | 0,54 | 0,52 |
| 2 | 0,70 | 0,57 | 0,50 | 0,46 | 0,42 | 0,40 |
| 3 | 0,62 | 0,50 | 0,42 | 0,38 | 0,36 | 0,34 |
| 4 | 0,58 | 0,46 | 0,38 | 0,35 | 0,32 | 0,30 |
| 5 | 0,54 | 0,42 | 0,36 | 0,32 | 0,30 | 0,28 |
| 6 | 0,52 | 0,40 | 0,34 | 0,30 | 0,28 | 0,26 |

### 6.17 K17 — Câbles en conduits non jointifs enterrés

La capture indique : un câble par conduit.

| Nombre de câbles | a = 0,25 m | a = 0,5 m | a = 1,0 m |
|---:|---:|---:|---:|
| 2 | 0,74 | 0,76 | 0,78 |
| 3 | 0,70 | 0,73 | 0,76 |
| 4 | 0,67 | 0,71 | 0,75 |
| 5 | 0,65 | 0,70 | 0,74 |
| 6 | 0,63 | 0,69 | 0,73 |

Les dispositions illustrées distinguent les câbles multiconducteurs et monoconducteurs. Ne pas extrapoler au-delà des configurations tabulées.

---

## 7. Facteur de symétrie fs — Article 523.4

### 7.1 Texte de contexte relevé

Pour une liaison triphasée réalisée avec plusieurs câbles unipolaires par phase, des tensions induites dues au champ magnétique produit par les câbles peuvent créer une disparité entre impédances et courants admissibles des câbles. Le respect de certaines conditions peut réduire ce phénomène.

La capture indique également :

- l’utilisation de ternes de câbles unipolaires (trois câbles unipolaires disposés en trèfle) doit être limitée à quatre ;
- au-delà de quatre câbles par phase et sans dépasser huit, des câbles unipolaires assemblés en torsades ou des câbles tripolaires doivent être utilisés ;
- les conditions de symétrie doivent être respectées afin que la formation des câbles ne pénalise pas le courant admissible.

### 7.2 Configurations illustrées

Les captures illustrent les valeurs suivantes en trèfle et en nappe :

| Nombre de câbles par phase | Configuration symétrique illustrée | fs indiqué |
|---:|---|---:|
| 2 | disposition illustrée en trèfle ou en nappe | 1 |
| 3 | disposition illustrée en trèfle ou en nappe | 0,8 |
| 4 | disposition illustrée en trèfle ou en nappe | 1 |

Le texte précise que le non-respect des conditions de symétrie indiquées dans les cas de 2, 3 et 4 câbles par phase impose l’utilisation de :

```text
fs = 0,8
```

### 7.3 Limites de fs

La capture ne fournit pas de méthode universelle pour calculer `fs` pour toute disposition possible.

Règles logicielles :

- ne pas déduire `fs` uniquement du nombre de câbles ;
- utiliser `fs` uniquement pour une disposition correspondant aux cas explicitement documentés ;
- si la disposition ou les conditions de symétrie ne peuvent pas être déterminées, demander une précision ou marquer le calcul partiel ;
- la manière exacte de combiner `fs` avec les facteurs K n’est pas explicitée dans les captures fournies ; afficher cette combinaison comme hypothèse si elle est implémentée, ou bloquer le calcul complet jusqu’à validation.

---

## 8. Sélection des tableaux et des facteurs

### 8.1 Entrées à prévoir

Entrées générales :

- matériau de l’âme : cuivre ou aluminium ;
- matériau isolant : PVC, PR ou EPR ;
- type de câble :
  - tripolaire ;
  - circuit de trois câbles unipolaires ;
- tension assignée si nécessaire à la sélection du tableau ;
- section nominale ;
- mode de pose :
  - à l’air ;
  - enterré ;
  - conduit ;
  - goulotte ou câble enfermé ;
  - caniveau ;
  - autre mode documenté ;
- courant demandé pour la recherche de section.

Entrées propres aux tableaux :

- 52J : disposition en trèfle ou en nappe ;
- K1 : température ambiante pour une pose à l’air ;
- K12 : température du sol pour une pose enterrée ;
- K13 : résistivité thermique du sol et type de câble ;
- K14 : profondeur de pose ;
- K3 à K9 : nombre de câbles ou groupes et configuration correspondante ;
- K5/K6 : espacement `e`, diamètre `d` et, le cas échéant, hauteur `h` ;
- K7 : nombre de couches et facteur de pose en simple couche applicable ;
- K8 : nombre de conduits par couche et nombre de couches ;
- K10 : géométrie du caniveau et encombrement total, sous réserve de clarifier la formule pour les câbles unipolaires ;
- K11 : type de tambour et nombre de spires ou couches ;
- K15 : nombre de circuits enterrés et espacement ;
- K16 : nombre de nappes et nombre de conduits jointifs par nappe ;
- K17 : nombre de câbles et espacement ;
- `fs` : nombre de câbles par phase et disposition réelle documentée.

Toute autre entrée nécessaire à un facteur ajouté ultérieurement doit être identifiée et documentée.

### 8.2 Sélection du tableau de courant

L’agent doit construire une règle de sélection explicite et testable à partir des critères disponibles :

- 52G : tripolaire, champ radial, PR/EPR, plage de tensions indiquée ;
- 52H : tripolaire, champ non radial, PR/EPR, 6/6 (7,2) kV ;
- 52I : tripolaire, champ non radial, PVC, 6/6 (7,2) kV ;
- 52J : trois câbles unipolaires, champ radial, PR/EPR, plage de tensions indiquée, choix trèfle/nappe.

Si plusieurs tableaux semblent compatibles ou si une donnée nécessaire manque, ne pas sélectionner arbitrairement : produire un état « impossible » et demander la donnée ou la règle manquante.

---

## 9. Applicabilité et combinaison des facteurs

Le tableau 52K identifie les facteurs associés aux modes de pose.

### 9.1 Pose à l’air libre

- K1 : température ambiante ;
- K2 : exposition directe au soleil ;
- K3 : simple couche sur murs, planchers ou tablettes non perforées ;
- K4 : simple couche au plafond ;
- K5 : simple couche sur tablettes perforées ;
- K6 : simple couche sur échelles, corbeaux ou treillis soudés ;
- K7 : pose en plusieurs couches ;
- K8 : conduits jointifs ;
- K9 : câbles enfermés ;
- K10 : caniveaux fermés, non remplis, affleurant le sol ;
- K11 : câbles souples sur tambour pour engins mobiles.

### 9.2 Pose dans le sol

- K12 : température du sol ;
- K13 : résistivité thermique du sol ;
- K14 : profondeur de pose ;
- K15 : nombre de circuits enterrés ;
- K16 : conduits jointifs enterrés ou noyés dans le béton ;
- K17 : conduits non jointifs enterrés.

### 9.3 Règles prudentes de sélection

- Ne pas appliquer tous les K par défaut.
- Sélectionner les facteurs selon la configuration réelle.
- Ne pas appliquer plusieurs facteurs de pose qui décrivent des configurations alternatives sans justification.
- La capture de K7 autorise explicitement le produit de K7 avec l’un des facteurs K3, K4, K5 ou K6 selon la situation.
- Les règles de combinaison complètes des autres facteurs ne sont pas entièrement documentées dans les captures.
- Si la combinaison applicable n’est pas sûre, afficher une question ou un avertissement et éviter de déclarer le calcul complet.
- La relation de `fs` avec les facteurs K doit être documentée avant de la présenter comme règle normative confirmée.

---

## 10. Gestion des valeurs hors table

Aucune règle générale d’interpolation ou d’arrondi n’est confirmée.

Par défaut :

- ne pas interpoler entre des valeurs tabulées ;
- ne pas extrapoler les courants au-delà des sections ;
- ne pas extrapoler un facteur au-delà des valeurs montrées ;
- signaler toute entrée hors du domaine documenté ;
- conserver les valeurs sources sans arrondi prématuré ;
- documenter l’arrondi d’affichage ;
- ne pas utiliser un facteur manquant comme égal à 1 ;
- un emplacement tabulaire vide signifie « non documenté / non disponible », pas zéro et pas interpolation permise.

---

## 11. Résultats et explication exigés

Pour chaque calcul, afficher :

1. le tableau de référence retenu ;
2. le type de câble, isolant, conducteur, section et mode de pose ;
3. le courant de référence en ampères ;
4. chaque facteur avec :
   - nom ;
   - valeur ;
   - condition qui a déclenché son application ;
   - référence au tableau ou à la formule ;
5. le facteur global ;
6. le courant corrigé en ampères ;
7. les hypothèses de calcul ;
8. les conditions de validité du tableau de référence ;
9. les facteurs pertinents non documentés ou non calculés ;
10. l’état du calcul : complet, partiel ou impossible ;
11. les avertissements et les limites d’interprétation.

Pour la recherche de section, afficher les sections examinées ou, au minimum, les sections immédiatement inférieure et retenue, avec leurs courants corrigés.

---

## 12. Questions ouvertes et données à vérifier

### 12.1 À vérifier avant intégration définitive

- édition exacte de la norme ;
- vérification cellule par cellule des tableaux 52G à 52J ;
- formulation « température ambiante maximum : 20 °C » pour la pose enterrée du tableau 52I ;
- libellé exact de K10 sur l’encombrement des câbles unipolaires en trèfle et en nappe ;
- règles générales de combinaison des facteurs ;
- combinaison de `fs` avec les facteurs K ;
- domaine d’application des formules K1 et K12 hors températures tabulées ;
- règles générales d’arrondi ;
- traitement des valeurs intermédiaires entre cases tabulées.

### 12.2 Facteurs ou données non documentés

- K2 : facteur soleil, valeur ou méthode complète absente ; référence IEC 60287 mentionnée ;
- toute configuration non représentée dans les tableaux ;
- valeurs non présentes dans les lignes ou colonnes tabulées ;
- méthode universelle de calcul de `fs` pour des dispositions autres que celles illustrées.

---

## 13. Tests et critères d’acceptation

L’implémentation est acceptable pour le périmètre documentaire lorsqu’elle respecte au minimum les points suivants.

### 13.1 Sélection de référence

- chaque combinaison de type de câble, isolant, tension, conducteur et pose sélectionne le tableau attendu ;
- toute combinaison non documentée est refusée ou signalée, jamais devinée.

### 13.2 Facteurs

- les valeurs sources sont stockées sans arrondi prématuré ;
- un facteur n’est appliqué que si ses conditions d’application sont satisfaites ;
- les configurations non tabulées ne sont pas interpolées ;
- un facteur nécessaire absent n’est jamais remplacé par 1 ;
- l’état partiel est déclenché pour K2 ou toute autre donnée nécessaire manquante ;
- les cas `fs` sont limités aux configurations explicitement documentées, sauf extension ultérieure validée.

### 13.3 Calcul de section

- seule une section tabulée peut être retenue ;
- la section minimale est la plus petite section disponible satisfaisant `courant_corrigé >= I_demande` ;
- si aucune section ne satisfait la demande, le logiciel le dit sans extrapoler.

### 13.4 Explicabilité

- le résultat contient les références, valeurs, facteurs, hypothèses et avertissements ;
- le statut complet/partiel/impossible est visible ;
- le logiciel indique que le calcul thermique n’est pas une validation complète du dimensionnement.

---

## 14. Avertissement à présenter à l’utilisateur

> Ce logiciel est une aide au calcul dans le périmètre des tableaux, valeurs et règles documentés. Il ne constitue ni une reproduction officielle de la norme ni une certification de conformité. Les données et règles intégrées doivent être vérifiées sur une copie autorisée et à jour de la norme, avec l’édition et les références conservées. Le calcul thermique ne remplace pas les autres vérifications nécessaires au dimensionnement électrique.

---

## 15. Résumé de l’état documentaire

### Confirmé par les captures fournies

- descriptions et valeurs visibles des tableaux 52G à 52J ;
- conditions de référence des tableaux ;
- tables de facteurs K1 à K17 ;
- certaines règles de combinaison, notamment K7 avec l’un des facteurs K3 à K6 ;
- configurations illustrées et valeurs de `fs` ;
- formule générale de calcul et besoin d’expliquer le résultat.

### Non entièrement confirmé

- combinaison exhaustive des facteurs ;
- méthode universelle pour `fs` ;
- règles d’interpolation, extrapolation et arrondi ;
- certains libellés et détails d’application de K10 et du tableau 52I ;
- édition exacte de la norme ;
- validation indépendante de la transcription des tableaux de courant.

### Conclusion d’usage

Un agent peut réaliser un prototype et couvrir les configurations explicitement documentées, à condition d’implémenter les états **complet**, **partiel** et **impossible**. Le logiciel ne doit pas prétendre couvrir les configurations absentes ni garantir une conformité normative complète avant vérification des points ouverts.