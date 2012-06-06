function Item(name, description)
{
    this.name = name;
    this.description = description;
}

//Retrieve items by their ID

function getItem(itemId)
{
    switch(itemId)
    {
    case 1001:
        return new Item("Boots of Speed", "UNIQUE Passive: Enhanced Movement 1 (does not stack with other Boots)");
    case 1004:
        return new Item("Faerie Charm", "+3 Mana Regen per 5 seconds");
    case 1005:
        return new Item("Meki Pendant", "+7 Mana Regen per 5 seconds");
    case 1006:
        return new Item("Rejuvenation Bead", "+8 Health Regen per 5 seconds");
    case 1007:
        return new Item("Regrowth Pendant", "+15 Health Regen per 5 seconds");
    case 1011:
        return new Item("Giant's Belt", "+430 Health");
    case 1018:
        return new Item("Cloak of Agility", "+18% Critical Strike Chance");
    case 1026:
        return new Item("Blasting Wand", "+40 Ability Power");
    case 1027:
        return new Item("Sapphire Crystal", "+200 Mana");
    case 1028:
        return new Item("Ruby Crystal", "+180 Health");
    case 1029:
        return new Item("Cloth Armor", "+18 Armor");
    case 1031:
        return new Item("Chain Vest", "+45 Armor");
    case 1033:
        return new Item("Null-Magic Mantle", "+24 Magic Resist");
    case 1036:
        return new Item("Long Sword", "+10 Attack Damage");
    case 1037:
        return new Item("Pickaxe", "+25 Attack Damage");
    case 1038:
        return new Item("B. F. Sword", "+45 Attack Damage");
    case 1042:
        return new Item("Dagger", "+15% Attack Speed");
    case 1043:
        return new Item("Recurve Bow", "+40% Attack Speed");
    case 1051:
        return new Item("Brawler's Gloves", "+8% Critical Strike Chance");
    case 1052:
        return new Item("Amplifying Tome", "+20 Ability Power");
    case 1053:
        return new Item("Vampiric Scepter", "+10% Life Steal");
    case 1054:
        return new Item("Doran's Shield", "+120 Health +10 Armor +8 Health Regen per 5 seconds");
    case 1055:
        return new Item("Doran's Blade", "+100 Health +10 Attack Damage +3% Life Steal");
    case 1056:
        return new Item("Doran's Ring", "+100 Health +15 Ability Power +5 Mana Regen per 5 seconds");
    case 1057:
        return new Item("Negatron Cloak", "+48 Magic Resist");
    case 1058:
        return new Item("Needlessly Large Rod", "+80 Ability Power");
    case 1062:
        return new Item("Prospector's Blade", "+20 Attack Damage +5% Life Steal  UNIQUE Passive: +200 Health (does not stack with other Prospector items)");
    case 1063:
        return new Item("Prospector's Ring", "+30 Ability Power +7 Mana Regen per 5 seconds  UNIQUE Passive: +200 Health (does not stack with other Prospector items)");
    case 2003:
        return new Item("Health Potion", "Click to Consume: Restores 150 Health over 15 seconds.");
    case 2004:
        return new Item("Mana Potion", "Click to Consume: Restores 100 Mana over 15 seconds.");
    case 2037:
        return new Item("Elixir of Fortitude", "Click to Consume: Grants 140-235 Health, based on champion level, and 10 Attack Damage for 4 minutes.");
    case 2038:
        return new Item("Elixir of Agility", "Click to Consume: Grants 12-22% Attack Speed, based on champion level, and 8% Critical Strike Chance for 4 minutes.");
    case 2039:
        return new Item("Elixir of Brilliance", "Click to Consume: Grants 20-40 Ability Power, based on champion level, and 10% Cooldown Reduction for 4 minutes.");
    case 2042:
        return new Item("Oracle's Elixir", "Click to Consume: Grants stealth detection until your champion dies.");
    case 2043:
        return new Item("Vision Ward", "Click to Consume: Places an invisible ward that reveals the surrounding area and stealthed units in the area for 3 minutes.");
    case 2044:
        return new Item("Sight Ward", "Click to Consume: Places an invisible ward that reveals the surrounding area for 3 minutes.");
    case 2047:
        return new Item("Oracle's Extract", "Click to Consume: Grants stealth detection for 5 minutes or until your champion dies.");
    case 3001:
        return new Item("Abyssal Scepter", "+70 Ability Power +57 Magic Resist  UNIQUE Aura: Reduces the Magic Resist of nearby enemy champions by 20.");
    case 3003:
        return new Item("Archangel's Staff", "+400 Mana +25 Mana Regen per 5 seconds +45 Ability Power  Passive: Grants Ability Power equal to 3% of...");
    case 3004:
        return new Item("Manamune", "+350 Mana +7 Mana Regen per 5 seconds +20 Attack Damage  UNIQUE Passive: Grants Attack Damage equal to...");
    case 3005:
        return new Item("Atma's Impaler", "+45 Armor +18% Critical Strike Chance  UNIQUE Passive: Gain Attack Damage equal to 2% of your maximum Health.");
    case 3006:
        return new Item("Berserker's Greaves", " +25% Attack Speed  UNIQUE Passive: Enhanced Movement 2 (does not stack with other Boots)");
    case 3009:
        return new Item("Boots of Swiftness", "UNIQUE Passive: Enhanced Movement 3 (does not stack with other Boots)");
    case 3010:
        return new Item("Catalyst the Protector", "+290 Health +325 Mana  UNIQUE Passive: On leveling up, restores 250 Health and 200 Mana over 8 seconds.");
    case 3020:
        return new Item("Sorcerer's Shoes", "+20 Magic Penetration  UNIQUE Passive: Enhanced Movement 2 (does not stack with other Boots)");
    case 3022:
        return new Item("Frozen Mallet", "+700 Health +20 Attack Damage  UNIQUE Passive: Your basic attacks slow your target's Movement Speed by 40% for 2.5 seconds (30% for ranged attacks).");
    case 3024:
        return new Item("Glacial Shroud", "+425 Mana +45 Armor  UNIQUE Passive: +15% Cooldown Reduction");
    case 3026:
        return new Item("Guardian Angel", "+68 Armor +38 Magic Resist  UNIQUE Passive: Revives your champion upon death, restoring 750 Health and 375 Mana (5 minute cooldown).");
    case 3027:
        return new Item("Rod of Ages", "+450 Health +525 Mana +60 Ability Power  Passive: Your champion gains 18 Health, 20 Mana, and 2 Ability Power every minute (up to 10 times).  UNIQUE Passive: On leveling up, restores 250 Health and 200 Mana over 8 seconds.");
    case 3028:
        return new Item("Chalice of Harmony", "+30 Magic Resist +7.5 Mana Regen per 5 seconds  UNIQUE Passive: Increases your Mana Regen by 1% per 1% Mana you are missing.");
    case 3031:
        return new Item("Infinity Edge", "+80 Attack Damage +25% Critical Strike Chance  UNIQUE Passive: Your critical strikes now deal 250% damage instead of 200%.");
    case 3035:
        return new Item("Last Whisper", "+40 Attack Damage  UNIQUE Passive: +40% Armor Penetration");
    case 3037:
        return new Item("Mana Manipulator", "UNIQUE Aura: Nearby allied champions gain 7.2 Mana Regen per 5 seconds.");
    case 3041:
        return new Item("Mejai's Soulstealer", "+20 Ability Power  UNIQUE Passive: Your champion gains 8 Ability Power per stack, receiving 2 stacks...");
    case 3044:
        return new Item("Phage", "+225 Health +18 Attack Damage  UNIQUE Passive: Your basic attacks have a 25% chance to slow your target's Movement Speed by 30% for 2.5 seconds.");
    case 3046:
        return new Item("Phantom Dancer", "+55% Attack Speed +30% Critical Strike Chance +15% Movement Speed");
    case 3047:
        return new Item("Ninja Tabi", "+25 Armor  UNIQUE Passive: Reduces the damage taken from non-turret basic attacks by 10%.  UNIQUE Passive: Enhanced Movement 2 (does not stack with other Boots)");
    case 3050:
        return new Item("Zeke's Herald", "+250 Health UNIQUE Passive: +15% Cooldown Reduction UNIQUE Aura: Grants nearby allied champions 12% Life Steal and 20% Attack Speed.");
    case 3057:
        return new Item("Sheen", "+250 Mana +25 Ability Power  UNIQUE Passive: After using an ability, your next basic attack deals bonus physical damage equal to your base Attack Damage (2 second cooldown). Does not stack with Trinity Force or Lich Bane.");
    case 3065:
        return new Item("Spirit Visage", "+30 Magic Resist +250 Health  UNIQUE Passive: +10% Cooldown Reduction  UNIQUE Passive: Increases your healing and regeneration effects on yourself by 15%.");
    case 3067:
        return new Item("Kindlegem", "+200 Health  UNIQUE Passive: +10% Cooldown Reduction");
    case 3068:
        return new Item("Sunfire Cape", "+450 Health +45 Armor  UNIQUE Passive: Deals 35 magic damage per second to nearby enemies.");
    case 3069:
        return new Item("Shurelya's Reverie", "+330 Health +30 Health Regen per 5 seconds +15 Mana Regen per 5 seconds  UNIQUE Passive: +15% Cooldown Reduction  UNIQUE Active: Nearby allied champions gain 40% Movement Speed for 3 seconds (60 second cooldown).");
    case 3070:
        return new Item("Tear of the Goddess", "+350 Mana +7 Mana Regen per 5 seconds  UNIQUE Passive: Each time you use an ability, your maximum Mana increases by 4 (3 second cooldown). Bonus caps at +1000 Mana. Does not stack with Archangel's Staff or Manamune.");
    case 3071:
        return new Item("The Black Cleaver", "+55 Attack Damage +30% Attack Speed  UNIQUE Passive: Your basic attacks reduce your target's Armor by 15 for 5 seconds (effect stacks up to 3 times).");
    case 3072:
        return new Item("The Bloodthirster", "+60 Attack Damage +12% Life Steal  Passive: Gains 1 stack per kill, up to a maximum of 40. Each stack...");
    case 3075:
        return new Item("Thornmail", "+100 Armor  UNIQUE Passive: On being hit by basic attacks, returns 30% of damage taken as magic damage.");
    case 3077:
        return new Item("Tiamat", "+50 Attack Damage +15 Health Regen per 5 seconds +5 Mana Regen per 5 seconds  Passive: Your basic attacks splash, dealing 50% area damage around the target (35% for ranged attacks).");
    case 3078:
        return new Item("Trinity Force", "+30 Attack Damage +30 Ability Power +30% Attack Speed +15% Critical Strike Chance +12% Movement Speed...");
    case 3082:
        return new Item("Warden's Mail", "+50 Armor +20 Health Regen per 5 seconds  UNIQUE Passive: 20% chance on being hit by basic attacks to slow the attacker's Movement and Attack Speeds by 35% for 3 seconds.");
    case 3083:
        return new Item("Warmog's Armor", "+920 Health +30 Health Regen per 5 seconds  Passive: Minion kills permanently grant 3.5 Health and .10...");
    case 3086:
        return new Item("Zeal", "+20% Attack Speed +10% Critical Strike Chance +8% Movement Speed");
    case 3089:
        return new Item("Rabadon's Deathcap", "+140 Ability Power  UNIQUE Passive: Increases Ability Power by 30%");
    case 3091:
        return new Item("Wit's End", "+40% Attack Speed +30 Magic Resist  UNIQUE Passive: Your basic attacks deal 42 bonus magic damage.  UNIQUE Passive: Your basic attacks increase your Magic Resist by 5 for 5 seconds (effect stacks up to 4 times).");
    case 3093:
        return new Item("Avarice Blade", "+12% Critical Strike Chance  UNIQUE Passive: Gain an additional 5 Gold every 10 seconds.");
    case 3096:
        return new Item("Philosopher's Stone", "+18 Health Regen per 5 seconds +8 Mana Regen per 5 seconds  UNIQUE Passive: Gain an additional 5 Gold every 10 seconds.");
    case 3097:
        return new Item("Emblem of Valor", "+25 Armor  UNIQUE Aura: Nearby allied Champions gain 10 Health Regen per 5 seconds.");
    case 3098:
        return new Item("Kage's Lucky Pick", "+25 Ability Power UNIQUE Passive: Gain an additional 5 Gold every 10 seconds.");
    case 3099:
        return new Item("Soul Shroud", "+520 Health  UNIQUE Aura: Nearby allied champions gain 10% Cooldown Reduction and 12 Mana Regen per 5 seconds.");
    case 3100:
        return new Item("Lich Bane", "+350 Mana +80 Ability Power +30 Magic Resist +7% Movement Speed  UNIQUE Passive: After using an ability,...");
    case 3101:
        return new Item("Stinger", "+40% Attack Speed  UNIQUE Passive: +10% Cooldown Reduction");
    case 3102:
        return new Item("Banshee's Veil", "+375 Health +375 Mana +50 Magic Resist  UNIQUE Passive: Gain a spell shield that blocks the next incoming enemy ability (45 second cooldown).");
    case 3105:
        return new Item("Aegis of the Legion", "+270 Health +18 Armor +24 Magic Resist  UNIQUE Aura: Nearby allied champions gain 12 Armor, 15 Magic Resist, and 8 Attack Damage.");
    case 3106:
        return new Item("Madred's Razors", "+15 Attack Damage +23 Armor  UNIQUE Passive: Your basic attacks against minions and monsters have a 20% chance to deal 300 bonus magic damage.");
    case 3108:
        return new Item("Fiendish Codex", "+30 Ability Power +7 Mana Regen per 5 seconds  UNIQUE Passive: +10% Cooldown Reduction");
    case 3109:
        return new Item("Force of Nature", "+76 Magic Resist +40 Health Regen per 5 seconds +8% Movement Speed  UNIQUE Passive: Restores 0.35% of your maximum Health every second.");
    case 3110:
        return new Item("Frozen Heart", "+99 Armor +500 Mana  UNIQUE Passive: +20% Cooldown Reduction  UNIQUE Aura: Reduces the Attack Speed of nearby enemies by 20%.");
    case 3111:
        return new Item("Mercury's Treads", "+25 Magic Resist  UNIQUE Passive: Enhanced Movement 2 (does not stack with other Boots)  UNIQUE Passive:...");
    case 3114:
        return new Item("Malady", "+25 Ability Power +50% Attack Speed  UNIQUE Passive: Your basic attacks deal 20 bonus magic damage and reduce the target's Magic Resist by 6 for 8 seconds (effect stacks up to 4 times).");
    case 3115:
        return new Item("Nashor's Tooth", "+50% Attack Speed +65 Ability Power +10 Mana Regen per 5 seconds  UNIQUE Passive: +25% Cooldown Reduction");
    case 3116:
        return new Item("Rylai's Crystal Scepter", "+500 Health +80 Ability Power  UNIQUE Passive: Dealing spell damage slows the target's Movement Speed by 35% for 1.5 seconds (15% for multi-target and damage-over-time spells).");
    case 3117:
        return new Item("Boots of Mobility", "UNIQUE Passive: Enhanced Movement 2, increases to Enhanced Movement 5 when out of combat for 5 seconds (does not stack with other Boots).");
    case 3123:
        return new Item("Executioner's Calling", "+18% Life Steal +15% Critical Strike Chance  UNIQUE Passive: Your basic attacks apply a mark to the target...");
    case 3124:
        return new Item("Guinsoo's Rageblade", "+35 Attack Damage +45 Ability Power  UNIQUE Passive: On basic attack or ability use, increases your Attack Speed by 4% and Ability Power by 6 for 5 seconds (effect stacks up to 8 times).");
    case 3126:
        return new Item("Madred's Bloodrazor", "+40 Attack Damage +40% Attack Speed +25 Armor  UNIQUE Passive: Your basic attacks deal bonus magic damage equal to 4% of the target's maximum Health.");
    case 3128:
        return new Item("Deathfire Grasp", "+60 Ability Power +10 Mana Regen per 5 seconds  UNIQUE Passive: +15% Cooldown Reduction  UNIQUE Active:...");
    case 3132:
        return new Item("Heart of Gold", "+250 Health  UNIQUE Passive: Gain an additional 5 Gold every 10 seconds.");
    case 3134:
        return new Item("The Brutalizer", "+25 Attack Damage  UNIQUE Passive: +10% Cooldown Reduction  UNIQUE Passive: +15 Armor Penetration");
    case 3135:
        return new Item("Void Staff", "+70 Ability Power  UNIQUE Passive: +40% Magic Penetration");
    case 3136:
        return new Item("Haunting Guise", "+25 Ability Power +200 Health  UNIQUE Passive: +20 Magic Penetration");
    case 3138:
        return new Item("Leviathan", "+180 Health  UNIQUE Passive: Your champion gains 32 Health per stack, receiving 2 stacks for a kill or...");
    case 3140:
        return new Item("Quicksilver Sash", "+56 Magic Resist  UNIQUE Active: Removes all debuffs from your champion (90 second cooldown).");
    case 3141:
        return new Item("Sword of the Occult", "+10 Attack Damage UNIQUE Passive: Your champion gains 5 Attack Damage per stack, receiving 2 stacks for...");
    case 3142:
        return new Item("Youmuu's Ghostblade", "+30 Attack Damage +15% Critical Strike Chance  UNIQUE Passive: +15% Cooldown Reduction  UNIQUE Passive:...");
    case 3143:
        return new Item("Randuin's Omen", "+350 Health +75 Armor +25 Health Regen per 5 seconds  UNIQUE Passive: +5% Cooldown Reduction  UNIQUE...");
    case 3144:
        return new Item("Bilgewater Cutlass", "+35 Attack Damage +15% Life Steal  UNIQUE Active: Deals 150 magic damage and slows the target champion's Movement Speed by 50% for 3 seconds (60 second cooldown).");
    case 3145:
        return new Item("Hextech Revolver", "+40 Ability Power  UNIQUE Passive: +12% Spell Vamp");
    case 3146:
        return new Item("Hextech Gunblade", "+40 Attack Damage +70 Ability Power +15% Life Steal  UNIQUE Passive: +20% Spell Vamp  UNIQUE Active: Deals 300 magic damage and slows the target champion's Movement Speed by 50% for 3 seconds (60 second cooldown).");
    case 3152:
        return new Item("Will of the Ancients", "+50 Ability Power  UNIQUE Aura: Grants nearby allied champions 30 Ability Power and 20% Spell Vamp");
    case 3154:
        return new Item("Wriggle's Lantern", "+23 Attack Damage +30 Armor +12% Life Steal  UNIQUE Passive: Your basic attacks against minions and monsters...");
    case 3155:
        return new Item("Hexdrinker", "+25 Attack Damage +30 Magic Resist  UNIQUE Passive: If you would take magic damage that would leave you at less than 30% Health, you first gain a shield that absorbs 250 magic damage for 3 seconds (60 second cooldown).");
    case 3156:
        return new Item("Maw of Malmortius", "+55 Attack Damage +36 Magic Resist  UNIQUE Passive: If you would take magic damage that would leave you...");
    case 3157:
        return new Item("Zhonya's Hourglass", "+100 Ability Power +50 Armor  UNIQUE Active: Places your champion into Stasis for 2 seconds, rendering you invulnerable and untargetable but unable to take any action (90 second cooldown).");
    case 3158:
        return new Item("Ionian Boots of Lucidity", "UNIQUE Passive: +15% Cooldown Reduction  UNIQUE Passive: Enhanced Movement 2 (does not stack with other Boots)");
    case 3165:
        return new Item("Morello's Evil Tome", "+75 Ability Power +12 Mana Regen per 5 seconds  UNIQUE Passive: +20% Cooldown Reduction");
    case 3170:
        return new Item("Moonflair Spellblade", "+50 Ability Power  UNIQUE Passive: +35 Tenacity (Tenacity reduces the duration of stuns, slows, taunts, fears, silences, blinds and immobilizes. Does not stack with other Tenacity items.)");
    case 3172:
        return new Item("Cloak and Dagger", "+20% Attack Speed +20% Critical Strike Chance  UNIQUE Passive: +35 Tenacity (Tenacity reduces the duration of stuns, slows, taunts, fears, silences, blinds and immobilizes. Does not stack with other Tenacity items.)");
    case 3173:
        return new Item("Eleisa's Miracle", "+25 Health Regen per 5 seconds +20 Mana Regen per 5 seconds  UNIQUE Passive: +35 Tenacity (Tenacity reduces...");
    case 3174:
        return new Item("Athene's Unholy Grail","+80 Ability Power +36 Magic Resist +15 Mana Regen per 5 seconds  UNIQUE Passive: 15% Cooldown Reduction  UNIQUE Passive: Restores 12% of your max Mana on Kill or Assist.  UNIQUE Passive: Increases your Mana Regen by 1% per 1% Mana you are missing. Does not stack with Chalice of Harmony.");
    case 3178:
        return new Item("Ionic Spark", "+50% Attack Speed +250 Health  UNIQUE Passive: Every fourth basic attack unleashes a chain lightning, dealing 110 magic damage to up to 4 targets.");
    case 3180:
        return new Item("Odyn's Veil", "+350 Health +350 Mana +50 Magic Resist  UNIQUE Passive: Reduces and stores 10% of the magic damage dealt...");
    case 3181:
        return new Item("Sanguine Blade", "+60 Attack Damage +15% Life Steal  UNIQUE Passive: Your basic attacks grant 5 Attack Damage and 1% Life Steal for 4 seconds (effect stacks up to 7 times).");
    case 3184:
        return new Item("Entropy", "+275 Health +70 Attack Damage UNIQUE Passive: Your basic attacks have a 25% chance to reduce your target's...");
    case 3185:
        return new Item("The Lightbringer", "+50% Attack Speed +20 Attack Damage  UNIQUE Passive: Your basic attacks grant vision of your target for 5 seconds.");
    case 3186:
        return new Item("Kitae's Bloodrazor", "+30 Attack Damage +40% Attack Speed  UNIQUE Passive: Your basic attacks deal magic damage equal to 2.5% of the target's maximum Health.");
    case 3187:
        return new Item("Hextech Sweeper", "+40 Ability Power +300 Health  UNIQUE Passive: +10% Cooldown Reduction  UNIQUE Passive: Dealing spell...");
    case 3190:
        return new Item("Locket of the Iron Solari", "+300 Health +35 Armor UNIQUE Aura: Nearby allied Champions gain 15 Health Regen per 5 seconds. UNIQUE Active: Shield yourself and nearby allies for 5 seconds, absorbing up to 50 (+10 per level) damage (60 second cooldown).");
    case 3196:
        return new Item("Augment: Power", "+3 Ability Power per level +220 Health +6 Health Regen per 5 seconds  Ability Augment: Power Transfer increases Viktor's Movement Speed by 30% for 3 seconds.");
    case 3197:
        return new Item("Augment: Gravity", "+3 Ability Power per level +200 Mana +10% Cooldown Reduction +5 Mana Regen per 5 seconds  Ability Augment: Gravity Field has an additional 30% cast range.");
    case 3198:
        return new Item("Augment: Death", "+3 Ability Power per level +45 Ability Power  Augment Ability: Death Ray sets fire to enemies, dealing 30% additional magic damage over 4 seconds.");
    case 3200:
        return new Item("The Hex Core", "+3 Ability Power per level. This item can be upgraded into one of three augments that enhance Viktor's basic abilities. Click the item in the store to discover its upgrades.");
    default:
        return new Item('Item ' + itemId, 'Unknown');
    }
}