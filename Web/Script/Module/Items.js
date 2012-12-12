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
        return new Item("Boots of Speed", "UNIQUE Passive - Enhanced Movement:\n+25 Movement Speed\n\n(Unique Passives with the same name don't stack.)");
    case 1004:
        return new Item("Faerie Charm", "+3 Mana Regen per 5 seconds");
    case 1005:
        return new Item("Meki Pendant", "+7 Mana Regen per 5 seconds");
    case 1006:
        return new Item("Rejuvenation Bead", "+5 Health Regen per 5 seconds");
    case 1007:
        return new Item("Regrowth Pendant", "+15 Health Regen per 5 seconds");
    case 1011:
        return new Item("Giant's Belt", "+400 Health");
    case 1018:
        return new Item("Cloak of Agility", "+15% Critical Strike Chance");
    case 1026:
        return new Item("Blasting Wand", "+40 Ability Power");
    case 1027:
        return new Item("Sapphire Crystal", "+200 Mana");
    case 1028:
        return new Item("Ruby Crystal", "+180 Health");
    case 1029:
        return new Item("Cloth Armor", "+15 Armor");
    case 1031:
        return new Item("Chain Vest", "+40 Armor");
    case 1033:
        return new Item("Null-Magic Mantle", "+20 Magic Resist");
    case 1036:
        return new Item("Long Sword", "+10 Attack Damage");
    case 1037:
        return new Item("Pickaxe", "+25 Attack Damage");
    case 1038:
        return new Item("B. F. Sword", "+45 Attack Damage");
    case 1039:
        return new Item("Hunter's Machete", "UNIQUE Passive - Butcher:\nDamage dealt to monsters increased by 10%.\n\nUNIQUE Passive - Rend:\nBasic attacks deal 10 bonus true damage to monsters.\n\n(Unique Passives with the same name don't stack.)");
    case 1042:
        return new Item("Dagger", "+12% Attack Speed");
    case 1043:
        return new Item("Recurve Bow", "+30% Attack Speed");
    case 1051:
        return new Item("Brawler's Gloves", "+8% Critical Strike Chance");
    case 1052:
        return new Item("Amplifying Tome", "+20 Ability Power");
    case 1053:
        return new Item("Vampiric Scepter", "+10 Attack Damage\n+10% Life Steal");
    case 1054:
        return new Item("Doran's Shield", "+100 Health\n+5 Armor\n+5 Health Regen per 5 seconds\n\n\nUNIQUE Passive:\nBlocks 6 damage from champion basic attacks.");
    case 1055:
        return new Item("Doran's Blade", "+80 Health\n+10 Attack Damage\n\nPassive: Your basic attacks restore 5 health each time they hit an enemy.");
    case 1056:
        return new Item("Doran's Ring", "+80 Health\n+15 Ability Power\n+3 Mana Regen per 5 seconds\n\nPassive: Restores 5 Mana when you kill an enemy unit.");
    case 1057:
        return new Item("Negatron Cloak", "+45 Magic Resist");
    case 1058:
        return new Item("Needlessly Large Rod", "+80 Ability Power");
    case 1062:
        return new Item("Prospector's Blade", "+20 Attack Damage\n+5% Life Steal\n\n\nUNIQUE Passive - Prospector:\n+200 Health\n\n(Unique Passives with the same name don't stack.)");
    case 1063:
        return new Item("Prospector's Ring", "+40 Ability Power\n+10 Mana Regeneration\n\n\nUNIQUE Passive - Prospector:\n+200 Health\n\n(Unique Passives with the same name don't stack.)");
    case 1080:
        return new Item("Spirit Stone", "+14 Health Regen per 5 seconds\n+7 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Butcher:\nDamage dealt to monsters increased by 20%.\n\nUNIQUE Passive - Rend:\nBasic attacks deal 10 bonus true damage to monsters.\n\n(Unique Passives or Actives with the same name don't stack.)");
    case 2003:
        return new Item("Health Potion", "Click to Consume: Restores 150 Health over 15 seconds.");
    case 2004:
        return new Item("Mana Potion", "Click to Consume: Restores 100 Mana over 15 seconds.");
    case 2037:
        return new Item("Elixir of Fortitude", "Click to Consume: Grants 120-235 Health, based on champion level, and 15 Attack Damage for 3 minutes.");
    case 2038:
        return new Item("Elixir of Agility", "Click to Consume:\nGrants 12-22% Attack Speed, based on champion level, and 8% Critical Strike Chance for 4 minutes.");
    case 2039:
        return new Item("Elixir of Brilliance", "Click to Consume: Grants 25-40 Ability Power, based on champion level, and 10% Cooldown Reduction for 3 minutes.");
    case 2040:
        return new Item("Ichor of Rage", "Click to Consume: Grants 20-42 Attack Damage based on champion level, 20-42% Attack Speed based on champion level, and 15% increased damage to Turrets for 4 minutes.");
    case 2041:
        return new Item("Crystalline Flask", "UNIQUE Active: Consumes a charge to restore 100 health and 40 mana over 10 seconds.\n\nUNIQUE Passive:\nStarts with 3 charges and refills each time you stop by your shop.");
    case 2042:
        return new Item("Oracle's Elixir", "Click to Consume: Grants detection of nearby invisible units for up to 5 minutes or until your champion dies.");
    case 2043:
        return new Item("Vision Ward", "Click to Consume: Places an invisible ward that reveals the surrounding area and invisible units in the area for 3 minutes.");
    case 2044:
        return new Item("Sight Ward", "Click to Consume: Places an invisible ward that reveals the surrounding area for 3 minutes.");
    case 2045:
        return new Item("Ruby Sightstone", "+300 Health\n\n\nUNIQUE Passive - Ward Refresh:\nStarts with 5 charges and refills each time you visit your shop.\nUNIQUE Active - Ghost Ward: Consumes a charge to place an invisible ward that reveals the surrounding area for 3 minutes. You may have a maximum of 3 wards placed from this item at once.");
    case 2047:
        return new Item("Oracle's Extract", "Click to Consume: Grants detection of nearby invisible units for up to 5 minutes or until your champion dies.");
    case 2048:
        return new Item("Ichor of Illumination", "Click to Consume: Grants 30-64 Ability Power based on champion level, 15% Cooldown Reduction, and a huge boost to mana and energy regeneration for 4 minutes.");
    case 2049:
        return new Item("Sightstone", "+100 Health\n\n\nUNIQUE Passive - Ward Refresh:\nStarts with 4 charges and refills each time you visit your shop.\nUNIQUE Active - Ghost Ward: Consumes a charge to place an invisible ward that reveals the surrounding area for 3 minutes. You may have a maximum of 2 wards placed from this item at once.");
    case 3001:
        return new Item("Abyssal Scepter", "+70 Ability Power\n+45 Magic Resist\n\nUNIQUE Aura: Reduces the Magic Resist of nearby enemies by 20.");
    case 3003:
        return new Item("Archangel's Staff", "+250 Mana\n+50 Ability Power\n+10 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Insight:\nGain Ability Power equal to 3% of your Maximum Mana.\n\nUNIQUE Passive - Mana Charge:\nEach time you cast a spell or spend Mana, you gain 5 maximum Mana (3 second cooldown). Bonus caps at +750 Mana.\n\nThis item transforms once it reaches +750 Mana, gaining a powerful mana shield active.");
    case 3004:
        return new Item("Manamune", "+250 Mana\n+10 Attack Damage\n+7 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Awe:\nGain Attack Damage equal to 2% of your maximum Mana.\n\nUNIQUE Passive - Mana Charge:\nEach time you attack, cast a spell or spend Mana, you gain 4 maximum Mana (3 second cooldown). Bonus caps at +750 Mana.\n\nTransforms into Muramana at 750 Bonus Mana.");
    case 3005:
        return new Item("Atma's Impaler", "+45 Armor\n+15% Critical Strike Chance\n\n\nUNIQUE Passive:\nGain Attack Damage equal to 1.5% of your maximum Health.");
    case 3006:
        return new Item("Berserker's Greaves", " +20% Attack Speed\n\n\nUNIQUE Passive - Enhanced Movement:\n+45 Movement Speed");
    case 3009:
        return new Item("Boots of Swiftness", "UNIQUE Passive - Enhanced Movement:\n+60 Movement Speed\n\nUNIQUE Passive - Slow Resist:\nMovement slowing effects are reduced by 25%.");
    case 3010:
        return new Item("Catalyst the Protector", "+200 Health\n+300 Mana\n\n\nUNIQUE Passive - Valor's Reward:\nOn leveling up, restores 150 Health and 200 Mana over 8 seconds.\n\n(Unique Passives with the same name don't stack.)");
    case 3020:
        return new Item("Sorcerer's Shoes", "+15 Magic Penetration\n\n\nUNIQUE Passive - Enhanced Movement:\n+45 Movement Speed");
    case 3022:
        return new Item("Frozen Mallet", "+700 Health\n+30 Attack Damage\n\n\nUNIQUE Passive - Icy:\nYour basic attacks slow your target's Movement Speed by 40% for 1.5 seconds (30% slow for ranged attacks).");
    case 3023:
        return new Item("Twin Shadows", "+50 Ability Power\n+30 Magic Resistance\n+5% Movement Speed\n\nUNIQUE Active - Hunt: Summons up to 2 invulnerable ghosts for 6 seconds to seek the two nearest enemy champions.  If they touch an enemy champion, they slow his Movement Speed by 40% and reveal him for  2.5 seconds - 120 second cooldown.");
    case 3024:
        return new Item("Glacial Shroud", "+300 Mana\n+40 Armor\n\n\nUNIQUE Passive:\n+15% Cooldown Reduction");
    case 3025:
        return new Item("Iceborn Gauntlet", "+40 Ability Power\n+500 Mana\n+60 Armor\n+15% Cooldown Reduction \n\n\nUNIQUE Passive - Spellblade:\nAfter using an ability, your next basic attack deals bonus physical damage equal to 125% of your base Attack Damage to surrounding enemies and creates a field for 3 seconds that slows enemies inside by 35% - 2 second cooldown.");
    case 3026:
        return new Item("Guardian Angel", "+50 Armor\n+30 Magic Resist\n\n\nUNIQUE Passive:\nRevives your champion upon death, restoring 30% of your Maximum Health and Mana - 5 minute cooldown.");
    case 3027:
        return new Item("Rod of Ages", "+450 Health\n+450 Mana\n+60 Ability Power\n\nPassive: This item gains 20 Health, 20 Mana, and 2 Ability Power every minute, up to 10 times.\n\nUNIQUE Passive - Valor's Reward:\nOn leveling up, restores 150 Health and 200 Mana over 8 seconds.");
    case 3028:
        return new Item("Chalice of Harmony", "+25 Magic Resist\n+7 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Mana Font:\nIncreases your Mana Regen by 1% per 1% Mana you are missing.\n\n(Unique Passives with the same name don't stack.)");
    case 3031:
        return new Item("Infinity Edge", "+70 Attack Damage\n+25% Critical Strike Chance\n\n\nUNIQUE Passive:\nYour critical strikes now deal 250% damage instead of 200%.");
    case 3035:
        return new Item("Last Whisper", "+40 Attack Damage\n\n\nUNIQUE Passive:\nYou ignore 35% of your opponent's armor.\n\n(Armor Penetration allows you to deal more physical damage to high Armored targets.  % Armor Penetration is applied before Flat Armor Penetration when calculating damage.)");
    case 3037:
        return new Item("Mana Manipulator", "UNIQUE Aura - Mana Warp: Nearby allied champions gain 6 Mana Regen per 5 seconds.\n\n(Unique Auras with the same name do not stack.)");
    case 3041:
        return new Item("Mejai's Soulstealer", "+20 Ability Power \n\n\nUNIQUE Passive:\nYour champion gains 8 Ability Power per stack, receiving 2 stacks for a kill or 1 stack for an assist (stacks up to 20). You lose a third of your stacks on death. At 20 stacks, your champion gains 15% Cooldown Reduction.");
    case 3042:
        return new Item("Muramana", "+1000 mana\n+20 attack damage\n+7 mana regen\nUnique Passive - Awe: Gain attack damage equal to 2% of your maximum mana.\nToggle: Your single target spells and attacks consume 3% of your current mana to deal 6% of your current mana as magic damage.");
    case 3044:
        return new Item("Phage", "+200 Health\n+20 Attack Damage\n\n\nUNIQUE Passive - Icy:\nYour basic attacks have a 25% chance to slow your target's Movement Speed by 30% for 2 seconds (20% slow for ranged attacks).");
    case 3046:
        return new Item("Phantom Dancer", "+50% Attack Speed\n+30% Critical Strike Chance\n+5% Movement Speed\n\n\nUNIQUE Passive:\nYour champion ignores unit collision.\n\n(A unit ignoring unit collision may walk through all units unhindered.)");
    case 3047:
        return new Item("Ninja Tabi", "+25 Armor\n\n\nUNIQUE Passive:\nBlocks 10% of the damage from champion basic attacks.\n\nUNIQUE Passive - Enhanced Movement:\n+45 Movement Speed");
    case 3050:
        return new Item("Zeke's Herald", "+250 Health\n+15% Cooldown Reduction\n\nUNIQUE Aura: Nearby allied champions gain 10% Life Steal and 20 Attack Damage.");
    case 3056:
        return new Item("Ohmwrecker", "+350 Health\n+300 Mana\n+55 Armor\n\nUNIQUE Active:  Prevents the closest enemy tower from attacking for 2.5 seconds - 120 second cooldown. This effect cannot be used against the same tower more than once every 7.5 seconds.");
    case 3057:
        return new Item("Sheen", "+200 Mana\n+25 Ability Power\n\n\nUNIQUE Passive - Spellblade:\nAfter using an ability, your next basic attack deals bonus physical damage equal to your base Attack Damage - 2 second cooldown.\n\n(Unique Passives with the same name don't stack.)");
    case 3060:
        return new Item("Banner of Command", "+50 Ability Power\n+30 Armor\n\nUNIQUE Aura - Valor: Nearby allies gain 10 Health Regen per 5 seconds and nearby allied minions deal 15% increased damage.\nUNIQUE Active - Promote: Transforms a nearby siege minion to a more powerful unit. You gain all the gold this unit earns - 180 second cooldown.");
    case 3065:
        return new Item("Spirit Visage", "+50 Magic Resist\n+200 Health\n+15% Cooldown Reduction\n\n\nUNIQUE Passive:\nIncreases your healing, regeneration and drain effects on yourself by 20%");
    case 3067:
        return new Item("Kindlegem", "+200 Health \n\n\nUNIQUE Passive:\n+10% Cooldown Reduction");
    case 3068:
        return new Item("Sunfire Cape", "+450 Health\n+45 Armor \n\n\nUNIQUE Passive:\nDeals 40 magic damage per second to nearby enemies.");
    case 3069:
        return new Item("Shurelya's Reverie", "+250 Health\n+10 Health Regen per 5 seconds\n+10 Mana Regen per 5 seconds\n\n\nUNIQUE Passive:\n+10% Cooldown Reduction\nUNIQUE Active: Nearby allies gain 40% Movement Speed for 3 seconds - 60 second cooldown.");
    case 3070:
        return new Item("Tear of the Goddess", "+250 Mana\n+7 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Mana Charge:\nEach time you cast a spell or spend Mana, your maximum Mana increases by 4 (3 second cooldown). Bonus caps at +750 Mana.\n\n(Unique Passives with the same name don't stack.)");
    case 3071:
        return new Item("The Black Cleaver", "+250 Health\n+50 Attack Damage\n+10% Cooldown Reduction\n+15 Armor Penetration\n\nPassive: Dealing physical damage to an enemy champion reduces their Armor by 7.5% for 4 seconds. This effect stacks up to 4 times.");
    case 3072:
        return new Item("The Bloodthirster", "+70 Attack Damage\n+12% Life Steal\n\nPassive: Gains 1 stack per kill, up to a maximum of 30. Each stack grants +1 Attack Damage and +0.2% Life Steal (max: +30 Attack Damage and +6% Life Steal). Half of the current stacks are lost upon death.");
    case 3074:
        return new Item("Ravenous Hydra (Melee Only)", "+75 Attack Damage\n+15 Health Regen per 5 seconds\n+10% Life Steal\n\nPassive: Damage dealt by this item works with Life Steal.\n\nUNIQUE Passive - Cleave:\nYour attacks deal up to 60% of your Attack Damage to units around your target - decaying down to 20% near the edge.\nUNIQUE Active - Crescent: Deals up to 100% of your Attack Damage to units around you - decaying down to 60% near the edge - 10 second cooldown.");
    case 3075:
        return new Item("Thornmail", "+100 Armor \n\n\nUNIQUE Passive:\nOn being hit by basic attacks, returns 30% of damage taken as magic damage.");
    case 3077:
        return new Item("Tiamat (Melee Only)", "+50 Attack Damage\n+15 Health Regen per 5 seconds\n\n\nUNIQUE Passive - Cleave:\nYour attacks deal up to 60% of your Attack Damage to units around your target - decaying down to 20% near the edge.\nUNIQUE Active - Crescent: Deals up to 100% of your Attack Damage to units around you - decaying down to 60% near the edge - 10 second cooldown.");
    case 3078:
        return new Item("Trinity Force", "+30 Attack Damage\n+30 Ability Power\n+30% Attack Speed\n+10% Critical Strike Chance\n+8% Movement Speed\n+250 Health\n+200 Mana\n\n\nUNIQUE Passive - Icy:\nYour basic attacks have a 25% chance to slow your target's Movement Speed by 30% for 2 seconds (20% for ranged.)\n\nUNIQUE Passive - Spellblade:\nAfter using an ability, your next basic attack deals bonus physical damage equal to 150% of your base Attack Damage (2 second cooldown).");
    case 3082:
        return new Item("Warden's Mail", "+50 Armor\n\n\nUNIQUE Passive - Cold Steel:\nIf you are hit by a basic attack, you slow the attacker's Attack Speed by 20% for 2 seconds.\n\n(Unique Passives with the same name don't stack.)");
    case 3083:
        return new Item("Warmog's Armor", "+1000 Health\n\n\nUNIQUE Passive:\nRestores 1.5% of your maxmium Health every 5 seconds.");
    case 3084:
        return new Item("Overlord's Bloodmail", "+850 Health\n\nUNIQUE Passive:\nOn kill or assist, gain 200 HP over 5 seconds.");
    case 3085:
        return new Item("Runaan's Hurricane (Ranged Only)", "+70% Attack Speed\n\n\nUNIQUE Passive:\nYour basic attacks fire minor bolts at 2 nearby targets, each dealing 10 + 50% of your Attack Damage. These apply on-hit effects.");
    case 3086:
        return new Item("Zeal", "+18% Attack Speed\n+10% Critical Strike Chance\n+5% Movement Speed");
    case 3087:
        return new Item("Statikk Shiv", "+40% Attack Speed\n+20% Critical Strike Chance\n+6% Movement Speed\n\n\nUNIQUE Passive:\nMoving and attacking build Static Charges. At 100 Charges, your next attack expends the Charges to deal 100 magic damage to up to 4 targets. This damage can critically strike.");
    case 3089:
        return new Item("Rabadon's Deathcap", "+120 Ability Power \n\n\nUNIQUE Passive:\nIncreases Ability Power by 25%");
    case 3090:
        return new Item("Wooglet's Witchcap", "+100 Ability Power\n+50 Armor \n\n\nUNIQUE Passive:\nIncreases Ability Power by 25%\nUNIQUE Active: Places your champion into Stasis for 2.5 seconds, rendering you invulnerable and untargetable but unable to take any action (90 second cooldown)");
    case 3091:
        return new Item("Wit's End", "+40% Attack Speed\n+20 Magic Resist\n\n\nUNIQUE Passive:\nYour basic attacks deal 42 bonus magic damage.\n\nUNIQUE Passive:\nYour basic attacks increase your Magic Resist by 5 for 5 seconds (effect stacks up to 4 times).");
    case 3092:
        return new Item("Shard of True Ice", "+45 Ability Power\n\n\nUNIQUE Passive - Lucky Shadow:\nGain an additional 4 Gold every 10 seconds.\nUNIQUE Aura - Mana Warp: Nearby allied champions gain 6 Mana Regen per 5 seconds.\nUNIQUE Active: Surrounds an ally with a blizzard for 4 seconds that slows nearby enemy movement speed by 30% - 60 second cooldown.");
    case 3093:
        return new Item("Avarice Blade", "+10% Critical Strike Chance\n\n\nUNIQUE Passive - Avarice:\nGain an additional 2 Gold every 10 seconds.\n\nUNIQUE Passive - Greed:\nGain an additional 2 Gold every kill.");
    case 3096:
        return new Item("Philosopher's Stone", "+7 Health Regen per 5 seconds\n+9 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Transmute:\nGain an additional 5 Gold every 10 seconds.");
    case 3097:
        return new Item("Emblem of Valor", "+20 Armor\n\nUNIQUE Aura: Nearby allied Champions gain 7 Health Regen per 5 seconds.");
    case 3098:
        return new Item("Kage's Lucky Pick", "+25 Ability Power\n\n\nUNIQUE Passive - Lucky Shadow:\nGain an additional 4 Gold every 10 seconds.\n\n(Unique Passives with the same name don't stack.)");
    case 3099:
        return new Item("Soul Shroud", "+520 Health\n\nUNIQUE Aura: Nearby allied champions gain 10% Cooldown Reduction and 12 Mana Regen per 5 seconds.");
    case 3100:
        return new Item("Lich Bane", "+80 Ability Power\n+250 Mana\n+5% Movement Speed\n\n\nUNIQUE Passive - Spellblade:\nAfter using an ability, your next basic attack deals bonus magic damage equal to 50 + 75% of your Ability Power - 2 second cooldown.\n\n(Unique Passives with the same name don't stack.)");
    case 3101:
        return new Item("Stinger", "+40% Attack Speed\n\n\nUNIQUE Passive:\n+10% Cooldown Reduction");
    case 3102:
        return new Item("Banshee's Veil", "+300 Health\n+300 Mana\n+45 Magic Resist\n\n\nUNIQUE Passive:\nGain a spell shield that blocks the next incoming enemy ability. This shield refreshes if you haven't taken damage from champions in 25 seconds.");
    case 3104:
        return new Item("Lord Van Damm's Pillager", "+40 Attack Damage\n+300 Health \nUNIQUE Passive:\n+10% Cooldown Reduction \nUNIQUE Passive:\n+10% Spell Vamp \nUNIQUE Passive:\n+20 Armor Penetration");
    case 3105:
        return new Item("Aegis of the Legion", "+250 Health\n+20 Armor\n+20 Magic Resist\n\nUNIQUE Aura - Legion: Nearby allies gain 10 Armor, 15 Magic Resist and 10 Health Regen per 5.\n\n(Unique Auras with the same name don't stack.)");
    case 3106:
        return new Item("Madred's Razors", "+25 Armor\n\n\nUNIQUE Passive - Maim:\nYour basic attacks against minions and monsters have a 25% chance to deal 300 bonus magic damage.\n\nUNIQUE Passive - Rend:\nBasic attacks deal 10 bonus true damage to monsters.");
    case 3107:
        return new Item("Runic Bulwark", "+400 Health\n+20 Armor\n+30 Magic Resist\n\nUNIQUE Aura - Legion: Nearby allies gain 10 Armor, 30 Magic Resist and 10 Health Regen per 5.\n\n(Unique Auras with the same name don't stack.)");
    case 3108:
        return new Item("Fiendish Codex", "+30 Ability Power\n+6 Mana Regen per 5 seconds\n\n\nUNIQUE Passive:\n+10% Cooldown Reduction");
    case 3109:
        return new Item("Force of Nature", "+76 Magic Resist +40 Health Regen per 5 seconds +8% Movement Speed\n\nUNIQUE Passive: Restores 0.35% of your maximum Health every second.");
    case 3110:
        return new Item("Frozen Heart", "+90 Armor\n+400 Mana\n+20% Cooldown Reduction\n\nUNIQUE Aura: Reduces the Attack Speed of nearby enemies by 20%.");
    case 3111:
        return new Item("Mercury's Treads", "+25 Magic Resist\n\n\nUNIQUE Passive - Enhanced Movement:\n+45 Movement Speed\n\nUNIQUE Passive - Tenacity:\nThe duration of stuns, slows, taunts, fears, silences, blinds and immobilizes are reduced by 35%.");
    case 3114:
        return new Item("Malady", "+25 Ability Power\n+45% Attack Speed\n\n\nUNIQUE Passive:\nYour basic attacks deal 15 + 10% of your Ability Power as bonus magic damage.\n\nUNIQUE Passive:\nYour basic attacks reduce enemy Magic Resistance by 4 for 8 seconds (effect stacks up to 7 times).");
    case 3115:
        return new Item("Nashor's Tooth", "+50% Attack Speed\n+65 Ability Power\n+10 Mana Regen per 5 seconds\n\n\nUNIQUE Passive:\n+20% Cooldown Reduction");
    case 3116:
        return new Item("Rylai's Crystal Scepter", "+500 Health\n+80 Ability Power \n\n\nUNIQUE Passive:\nDealing spell damage slows the target's Movement Speed by 35% for 1.5 seconds (15% for multi-target and damage-over-time spells).");
    case 3117:
        return new Item("Boots of Mobility", "UNIQUE Passive - Enhanced Movement:\n+45 Movement Speed. Increases to +105 Movement Speed when out of combat for 5 seconds.");
    case 3122:
        return new Item("Wicked Hatchet", "+20 Attack Damage\n+18% Critical Strike Chance\n\n\nUNIQUE Passive:\nYour basic attacks inflict Grievous Wounds on enemy champions, causing 50% reduced healing and regeneration for 1.5 seconds.");
    case 3123:
        return new Item("Executioner's Calling", "+25 Attack Damage\n+15% Critical Strike Chance\n\n\nUNIQUE Passive:\nYour basic attacks inflict Grievous Wounds on enemy champions for 1.5 seconds.\n\n(Grievous wounds reduces healing and regeneration by 50%)");
    case 3124:
        return new Item("Guinsoo's Rageblade", "+30 Attack Damage\n+40 Ability Power\n\nPassive: Your basic attacks or spellcasts grant you 4% Attack Speed and 4 Ability Power for 8 seconds. This bonus stacks up to 8 times.\n\nUNIQUE Passive:\nFalling below 50% health grants you 20% Attack Speed, 10% Life Steal and 10% Spell Vamp until you exit combat - 30 second cooldown.");
    case 3126:
        return new Item("Madred's Bloodrazor", "+40 Attack Damage +40% Attack Speed +25 Armor\n\nUNIQUE Passive: Your basic attacks deal bonus magic damage equal to 4% of the target's maximum Health.");
    case 3128:
        return new Item("Deathfire Grasp", "+100 Ability Power\n+15% Cooldown Reduction\n\nUNIQUE Active: Deals 15% of target champion's maximum Health in Magic Damage then amplifies all magic damage they take by 20% for 4 seconds - 60 second cooldown.");
    case 3131:
        return new Item("Sword of the Divine", "+45% Attack Speed\n\nPassive: This item does not grant any Attack Speed while on cooldown. Champion kills reduce the current cooldown by 50%.\nUNIQUE Active: Gain 100% Attack Speed and 100% Critical Strike Chance for 3 seconds or 3 critical strikes - 60 second cooldown.");
    case 3132:
        return new Item("Heart of Gold", "+250 Health\n\nUNIQUE Passive: Gain an additional 5 Gold every 10 seconds.");
    case 3134:
        return new Item("The Brutalizer", "+25 Attack Damage\n\n\nUNIQUE Passive:\n+10% Cooldown Reduction\n\nUNIQUE Passive:\n+15 Armor Penetration\n\n(Armor Penetration allows you to deal more physical damage to high Armored targets. % Armor Penetration is applied before Flat Armor Penetration when calculating damage.)");
    case 3135:
        return new Item("Void Staff", "+70 Ability Power\n\n\nUNIQUE Passive:\nYou ignore 35% of your opponent's Magic Resistance.\n\n(Magic Penetration helps you deal more magic damage to enemies with high Magic Resistance. % Magic Penetration is applied before Flat Magic Penetration when calculating damage.)");
    case 3136:
        return new Item("Haunting Guise", "+25 Ability Power\n+200 Health\n\n\nUNIQUE Passive - Eyes of Pain:\n+15 Magic Penetration.\n\n(Magic Penetration helps you deal more magic damage to enemies with high Magic Resistance. % Magic Penetration is applied before Flat Magic Penetration when calculating damage. Unique Passives with the same name do not stack.)");
    case 3138:
        return new Item("Leviathan", "+180 Health\n\nUNIQUE Passive: Your champion gains 32 Health per stack, receiving 2 stacks for a kill or...");
    case 3139:
        return new Item("Mercurial Scimitar", "+60 Attack Damage\n+45 Magic Resist\n\nUNIQUE Active - Quicksilver: Removes all debuffs from your champion. If your champion is Melee, this item also grants 50% bonus Movement Speed for 1 second - 90 second cooldown.");
    case 3140:
        return new Item("Quicksilver Sash", "+45 Magic Resist\n\nUNIQUE Active - Quicksilver: Removes all debuffs from your champion - 90 second cooldown.");
    case 3141:
        return new Item("Sword of the Occult", "+10 Attack Damage\n\n\nUNIQUE Passive:\nYour champion gains 5 Attack Damage per stack, receiving 2 stacks for a kill or 1 stack for an assist (stacks up to 20). You lose a third of your stacks on death. At 20 stacks, your champion's Movement Speed is increased by 15%.");
    case 3142:
        return new Item("Youmuu's Ghostblade", "+30 Attack Damage\n+15% Critical Strike Chance\n+10% Cooldown Reduction\n+20 Armor Penetration\n\nUNIQUE Active: Gain 20% Movement Speed and 40% Attack Speed for 6 seconds (4 seconds if you're ranged) - 45 second cooldown.");
    case 3143:
        return new Item("Randuin's Omen", "+500 Health\n+70 Armor\n\n\nUNIQUE Passive - Cold Steel:\nIf you are hit by a basic attack, you slow the attacker's Attack Speed by 20% and their Movement Speed by 10% for 1.5 seconds.\nUNIQUE Active: Slows the Movement Speed of nearby enemy units by 35% for 2 seconds + 1 second for every 100 Armor and Magic Resistance you have - 60 second cooldown.");
    case 3144:
        return new Item("Bilgewater Cutlass", "+40 Attack Damage\n+10% Life Steal\n\nUNIQUE Active: Deals 150 magic damage and slows the target champion's Movement Speed by 30% for 2 seconds - 60 second cooldown.");
    case 3145:
        return new Item("Hextech Revolver", "+40 Ability Power\n\n\nUNIQUE Passive:\n+12% Spell Vamp\n\n(Spell Vamp: Ability damage heals you for a percentage of the damage dealt. Area of Effect spells benefit one-third as much from Spell Vamp.)");
    case 3146:
        return new Item("Hextech Gunblade", "+45 Attack Damage\n+65 Ability Power\n+10% Life Steal\n+20% Spell Vamp\n\n\nUNIQUE Passive:\nYour basic attacks and single target spells against champions reduce the cooldown of this item by 3 seconds.\nUNIQUE Active: Deals 150 + 40% of your Ability Power as magic damage and slows target champion's Movement Speed by 40% for 2 seconds - 60 second cooldown.");
    case 3151:
        return new Item("Liandry's Torment", "+70 Ability Power\n+200 Health\n\n\nUNIQUE Passive - Eyes of Pain:\n+15 Magic Penetration\n\nUNIQUE Passive:\nDealing spell damage burns enemies for 5% of their current Health as magic damage over 3 seconds. If their movement is impaired, they take double damage from this effect.\n\n(Half duration for multi-target or periodic effects, 300 max vs. monsters.)");
    case 3152:
        return new Item("Will of the Ancients", "+50 Ability Power\n\nUNIQUE Aura: Grants nearby allied champions 30 Ability Power and 20% Spell Vamp");
    case 3153:
        return new Item("Blade of the Ruined King", "+40 Attack Damage\n+10% Life Steal\n\n\nUNIQUE Passive:\nYour attacks deal 4% of the target's current health in physical damage and heal you for half the amount (120 max vs. minions).\nUNIQUE Active: Drains target champion, dealing 150 physical damage plus 50% of your attack damage and healing you by the same amount. Additionally, you steal 30% of their movement speed for 2 seconds (60 second cooldown).");
    case 3154:
        return new Item("Wriggle's Lantern", "+15 Attack Damage\n+30 Armor\n+10% Life Steal\n\n\nUNIQUE Passive - Maim:\nYour basic attacks against minions and monsters have a 25% chance to deal 500 additional magic damage.\nUNIQUE Active: Places an invisible Sight Ward that reveals the surrounding area for 3 minutes - 3 minute cooldown.");
    case 3155:
        return new Item("Hexdrinker", "+25 Attack Damage\n+25 Magic Resist\n\n\nUNIQUE Passive - Lifeline:\nIf you would take magic damage that would leave you at less than 30% Health, you first gain a shield that absorbs 250 magic damage for 5 seconds - 90 second cooldown.\n\n(Unique Passives with the same name don't stack.)");
    case 3156:
        return new Item("Maw of Malmortius", "+55 Attack Damage\n+36 Magic Resist\n\n\nUNIQUE Passive:\nGain +1 Attack Damage for every 2.5% of your Maximum Health you are missing.\n\nUNIQUE Passive - Lifeline:\nIf you would take magic damage that would leave you at less than 30% Health, you first gain a shield that absorbs 400 magic damage for 5 seconds - 90 second cooldown.");
    case 3157:
        return new Item("Zhonya's Hourglass", "+100 Ability Power\n+50 Armor \n\nUNIQUE Active - Stasis: Your champion becomes invulnerable and untargetable but unable to take any actions for 2.5 seconds - 90 second cooldown.");
    case 3158:
        return new Item("Ionian Boots of Lucidity", "UNIQUE Passive:\n+15% Cooldown Reduction\n\nUNIQUE Passive - Enhanced Movement:\n+45 Movement Speed\n\n\"This item is dedicated in honor of Ionia's victory over Noxus in the Rematch for the Southern Provinces on 10 December, 20 CLE.\"");
    case 3159:
        return new Item("Grez's Spectral Lantern", "+25 Attack Damage\n+20 Armor\n+12% Life Steal\n\n\nUNIQUE Passive:\nYour basic attacks against minions and monsters have a 20% chance to deal 200 bonus magic damage.\nUNIQUE Active: A stealth-detecting mist grants vision in the target area for 10 seconds (60 second cooldown).");
    case 3165:
        return new Item("Morellonomicon", "+75 Ability Power\n+12 Mana Regen per 5 seconds\n+20% Cooldown Reduction\n\n\nUNIQUE Passive:\nDealing magic damage to an enemy champion below 40% health inflicts Grievous Wounds to them for 4 seconds.\n\n(Grievous Wounds reduces incoming healing and regeneration effects by 50%)");
    case 3166:
        return new Item("#<Item:0x467298>", "+2 Attack Damage per level\n\n\nUNIQUE Passive:\nRengar collects trophies when killing Champions and gains bonus effects based on how many trophies he has. Kills and assists grant 1 trophy, and 1 trophy is lost on death.\n3 Trophies: +10 Armor Penetration, +5% Cooldown Reduction.\n6 Trophies: +25 Movement Speed.\n9 Trophies: Rengar's leap gains 150 bonus range.\n14 Trophies: Thrill of the Hunt's duration is increased by 3 seconds. Additionally, Rengar's next ability used after activating Thrill of the Hunt generates 1 bonus Ferocity.");
    case 3167:
        return new Item("#<Item:0x465498>", "+2 Attack Damage per level\n\n\nUNIQUE Passive:\nRengar collects trophies when killing Champions and gains bonus effects based on how many trophies he has. Kills and assists grant 1 trophy, and 1 trophy is lost on death.\n3 Trophies: +10 Armor Penetration, +5% Cooldown Reduction.\n6 Trophies: +25 Movement Speed.\n9 Trophies: Rengar's leap gains 150 bonus range.\n14 Trophies: Thrill of the Hunt's duration is increased by 3 seconds. Additionally, Rengar's next ability used after activating Thrill of the Hunt generates 1 bonus Ferocity.");
    case 3168:
        return new Item("#<Item:0x463770>", "+2 Attack Damage per level\n\n\nUNIQUE Passive:\nRengar collects trophies when killing Champions and gains bonus effects based on how many trophies he has. Kills and assists grant 1 trophy, and 1 trophy is lost on death.\n3 Trophies: +10 Armor Penetration, +5% Cooldown Reduction.\n6 Trophies: +25 Movement Speed.\n9 Trophies: Rengar's leap gains 150 bonus range.\n14 Trophies: Thrill of the Hunt's duration is increased by 3 seconds. Additionally, Rengar's next ability used after activating Thrill of the Hunt generates 1 bonus Ferocity.");
    case 3169:
        return new Item("#<Item:0x461a00>", "+2 Attack Damage per level\n\n\nUNIQUE Passive:\nRengar collects trophies when killing Champions and gains bonus effects based on how many trophies he has. Kills and assists grant 1 trophy, and 1 trophy is lost on death.\n3 Trophies: +10 Armor Penetration, +5% Cooldown Reduction.\n6 Trophies: +25 Movement Speed.\n9 Trophies: Rengar's leap gains 150 bonus range.\n14 Trophies: Thrill of the Hunt's duration is increased by 3 seconds. Additionally, Rengar's next ability used after activating Thrill of the Hunt generates 1 bonus Ferocity.");
    case 3170:
        return new Item("Moonflair Spellblade", "+50 Ability Power\n\nUNIQUE Passive: +35 Tenacity (Tenacity reduces the duration of stuns, slows, taunts, fears, silences, blinds and immobilizes. Does not stack with other Tenacity items.)");
    case 3172:
        return new Item("Cloak and Dagger", "+20% Attack Speed +20% Critical Strike Chance\n\nUNIQUE Passive: +35 Tenacity (Tenacity reduces the duration of stuns, slows, taunts, fears, silences, blinds and immobilizes. Does not stack with other Tenacity items.)");
    case 3172:
        return new Item("Zephyr", "+20 Attack Damage\n+50% Attack Speed\n+10% Movement Speed\n+10% Cooldown Reduction\n\n\nUNIQUE Passive - Tenacity:\nThe duration of stuns, slows, taunts, fears, silences, blinds and immobilizes are reduced by 35%.\n\n(Unique Passives with the same name do not stack.)");
    case 3173:
        return new Item("Eleisa's Miracle", "+10 Health Regen per 5 seconds\n+15 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Aid:\n Your Summoner Heal, Revive and Clairvoyance cooldowns are reduced by 20%.\n\nUNIQUE Passive - Eleisa's Blessing:\nIf you gain 3 levels with this item, you gain all the effects of this item permanently and this item is consumed.");
    case 3174:
        return new Item("Athene's Unholy Grail", "+60 Ability Power\n+40 Magic Resist\n+15 Mana Regen per 5 seconds\n\n\nUNIQUE Passive:\n15% Cooldown Reduction\n\nUNIQUE Passive:\nRestores 12% of your max Mana on Kill or Assist.\n\nUNIQUE Passive - Mana Font:\nIncreases your Mana Regen by 1% per 1% Mana you are missing.");
    case 3178:
        return new Item("Ionic Spark", "+50% Attack Speed +250 Health\n\nUNIQUE Passive: Every fourth basic attack unleashes a chain lightning, dealing 110 magic damage to up to 4 targets.");
    case 3180:
        return new Item("Odyn's Veil", "+350 Health\n+350 Mana\n+50 Magic Resist\n\n\nUNIQUE Passive:\nReduces and stores 10% of the magic damage dealt to your champion. \nUNIQUE Active: Deals 200 + (stored magic) [max: 400] magic damage to nearby enemy units (90 second cooldown).");
    case 3181:
        return new Item("Sanguine Blade", "+60 Attack Damage\n+15% Life Steal\n\n\nUNIQUE Passive:\nYour basic attacks grant 5 Attack Damage and 1% Life Steal for 4 seconds (effect stacks up to 7 times).");
    case 3184:
        return new Item("Entropy", "+275 Health\n+70 Attack Damage\n\n\nUNIQUE Passive - Icy:\nYour basic attacks have a 25% chance to reduce your target's Movement Speed by 30% for 2.5 seconds.\nUNIQUE Active: For the next 5 seconds, your basic attacks reduce your target's Movement Speed by 30% and deal 80 true damage over 2.5 seconds (60 second cooldown).");
    case 3185:
        return new Item("The Lightbringer", "+50% Attack Speed\n+20 Attack Damage \n\n\nUNIQUE Passive:\nYour basic attacks grant vision of your target for 5 seconds.");
    case 3186:
        return new Item("Kitae's Bloodrazor", "+30 Attack Damage\n+40% Attack Speed \n\n\nUNIQUE Passive:\nYour basic attacks deal magic damage equal to 2.5% of the target's maximum Health.");
    case 3187:
        return new Item("Hextech Sweeper", "+50 Ability Power\n+300 Health\n\n\nUNIQUE Passive:\n+10% Cooldown Reduction\n\nUNIQUE Passive:\n+10% Movement Speed\nUNIQUE Active: A stealth-detecting mist grants vision in the target area for 10 seconds (60 second cooldown).");
    case 3188:
        return new Item("Blackfire Torch", "+70 Ability Power\n+250 Health \n\n\nUNIQUE Passive:\n+15% Cooldown Reduction \n\nUNIQUE Passive - Eyes of Pain:\n+15 Magic Penetration \n\nUNIQUE Passive:\nYour spells burn for an additional 3.5% of the target champion's maximum Health in magic damage over 2 seconds. Each second burned consumes a charge. Up to 18 charges are generated when not in use.\n\n(Magic Penetration helps you deal more magic damage to enemies with high Magic Resistance. % Magic Penetration is applied before Flat Magic Penetration when calculating damage. Unique Passives with the same name do not stack.)");
    case 3190:
        return new Item("Locket of the Iron Solari", "+425 Health\n+35 Armor\n+10% Cooldown Reduction\n+10 Health Regen per 5\n\nUNIQUE Active: Shield yourself and nearby allied champions for 5 seconds, absorbing up to 50 + 10 per level damage - 60 second cooldown.");
    case 3196:
        return new Item("Augment: Power", "+3 Ability Power per level\n+220 Health\n+6 Health Regen per 5 seconds\n\n\nAbility Augment:\nPower Transfer increases Viktor's Movement Speed by 30% for 3 seconds.");
    case 3197:
        return new Item("Augment: Gravity", "+3 Ability Power per level\n+200 Mana\n+10% Cooldown Reduction\n+5 Mana Regen per 5 seconds\n\n\nAbility Augment:\nGravity Field has an additional 30% cast range.");
    case 3198:
        return new Item("Augment: Death", "+3 Ability Power per level\n+45 Ability Power\n\n\nAugment Ability:\nDeath Ray sets fire to enemies, dealing 30% additional magic damage over 4 seconds.");
    case 3206:
        return new Item("Spirit of the Spectral Wraith", "+40 Ability Power\n+10 Mana Regen per 5 seconds\n\n\nUNIQUE Passive:\n+20% Spell Vamp\n\nUNIQUE Passive:\n+10% Cooldown Reduction\n\nUNIQUE Passive - Butcher:\nDamage dealt to monsters increased by 25%.");
    case 3207:
        return new Item("Spirit of the Ancient Golem", "+500 Health\n+30 Armor\n+14 Health Regen per 5 seconds\n+7 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Butcher:\nDamage dealt to monsters increased by 25%.\n\nUNIQUE Passive - Tenacity:\nThe durations of stuns, slows, taunts, fears, silences, blinds, and immobilizes are reduced by 35%.");
    case 3209:
        return new Item("Spirit of the Elder Lizard", "+50 Attack Damage\n+14 Health Regen per 5 seconds\n+7 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Butcher:\nDamage dealt to monsters increased by 25%.\n\nUNIQUE Passive - Incinerate:\nBasic attacks and non-periodic spell damage burn the target for 15-66 (based on level) true damage over 3 seconds.");
    case 3222:
        return new Item("Mikael's Crucible", "+300 Mana\n+40 Magic Resist\n+9 Mana Regen per 5 seconds\n\n\nUNIQUE Passive - Mana Font:\nIncreases your Mana Regen by 1% per 1% Mana you are missing.\nUNIQUE Active: Removes all stuns, roots, taunts, fears, silences and slows on an ally and heals them for 150 + 15% of their missing health - 180 second cooldown.");
    case 3250:
        return new Item("Enchantment: Homeguard", "Enchants these boots to have the Homeguard bonus.\n\n\nUNIQUE Passive - Homeguard:\nWhen you are at fountain and have been out of combat for 8 seconds you gain full health, mana and a 200% movement speed boost that decays over 8 seconds. You lose the movement speed when you enter combat.");
    case 3251:
        return new Item("Enchantment: Captain", "Enchants these boots to have the Captain bonus.\n\n\nUNIQUE Passive - Captain:\nAllied champions running towards you gain a 8% movement speed boost.  Nearby minions gain 25% additional movement speed.");
    case 3252:
        return new Item("Enchantment: Furor", "Enchants these boots to have the Furor bonus.\n\n\nUNIQUE Passive - Furor:\nWhenever you deal damage with a single target attack or spell, you gain 12% bonus movement speed that decays over 2 seconds.");
    case 3253:
        return new Item("Enchantment: Distortion", "Enchants these boots to have the Distortion bonus.\n\n\nUNIQUE Passive - Distortion:\nYour Teleport, Flash and Ghost summoner spell cooldowns are reduced by 25%.");
    case 3254:
        return new Item("Enchantment: Alacrity", "Enchants these boots to have the Alacrity bonus.\n\n\nUNIQUE Passive - Alacrity:\nYou gain +15 Movement Speed.");
    case 3255:
        return new Item("Enchantment: Homeguard", "Enchants these boots to have the Homeguard bonus.\n\n\nUNIQUE Passive - Homeguard:\nWhen you are at fountain and have been out of combat for 8 seconds you gain full health, mana and a 200% movement speed boost that decays over 8 seconds. You lose the movement speed when you enter combat.");
    case 3256:
        return new Item("Enchantment: Captain", "Enchants these boots to have the Captain bonus.\n\n\nUNIQUE Passive - Captain:\nAllied champions running towards you gain a 8% movement speed boost.  Nearby minions gain 25% additional movement speed.");
    case 3257:
        return new Item("Enchantment: Furor", "Enchants these boots to have the Furor bonus.\n\n\nUNIQUE Passive - Furor:\nWhenever you deal damage with a single target attack or spell, you gain 12% bonus movement speed that decays over 2 seconds.");
    case 3258:
        return new Item("Enchantment: Distortion", "Enchants these boots to have the Distortion bonus.\n\n\nUNIQUE Passive - Distortion:\nYour Teleport, Flash and Ghost summoner spell cooldowns are reduced by 25%.");
    case 3259:
        return new Item("Enchantment: Alacrity", "Enchants these boots to have the Alacrity bonus.\n\n\nUNIQUE Passive - Alacrity:\nYou gain +15 Movement Speed.");
    case 3260:
        return new Item("Enchantment: Homeguard", "Enchants these boots to have the Homeguard bonus.\n\n\nUNIQUE Passive - Homeguard:\nWhen you are at fountain and have been out of combat for 8 seconds you gain full health, mana and a 200% movement speed boost that decays over 8 seconds. You lose the movement speed when you enter combat.");
    case 3261:
        return new Item("Enchantment: Captain", "Enchants these boots to have the Captain bonus.\n\n\nUNIQUE Passive - Captain:\nAllied champions running towards you gain a 8% movement speed boost.  Nearby minions gain 25% additional movement speed.");
    case 3262:
        return new Item("Enchantment: Furor", "Enchants these boots to have the Furor bonus.\n\n\nUNIQUE Passive - Furor:\nWhenever you deal damage with a single target attack or spell, you gain 12% bonus movement speed that decays over 2 seconds.");
    case 3263:
        return new Item("Enchantment: Distortion", "Enchants these boots to have the Distortion bonus.\n\n\nUNIQUE Passive - Distortion:\nYour Teleport, Flash and Ghost summoner spell cooldowns are reduced by 25%.");
    case 3264:
        return new Item("Enchantment: Alacrity", "Enchants these boots to have the Alacrity bonus.\n\n\nUNIQUE Passive - Alacrity:\nYou gain +15 Movement Speed.");
    case 3265:
        return new Item("Enchantment: Homeguard", "Enchants these boots to have the Homeguard bonus.\n\n\nUNIQUE Passive - Homeguard:\nWhen you are at fountain and have been out of combat for 8 seconds you gain full health, mana and a 200% movement speed boost that decays over 8 seconds. You lose the movement speed when you enter combat.");
    case 3266:
        return new Item("Enchantment: Captain", "Enchants these boots to have the Captain bonus.\n\n\nUNIQUE Passive - Captain:\nAllied champions running towards you gain a 8% movement speed boost.  Nearby minions gain 25% additional movement speed.");
    case 3267:
        return new Item("Enchantment: Furor", "Enchants these boots to have the Furor bonus.\n\n\nUNIQUE Passive - Furor:\nWhenever you deal damage with a single target attack or spell, you gain 12% bonus movement speed that decays over 2 seconds.");
    case 3268:
        return new Item("Enchantment: Distortion", "Enchants these boots to have the Distortion bonus.\n\n\nUNIQUE Passive - Distortion:\nYour Teleport, Flash and Ghost summoner spell cooldowns are reduced by 25%.");
    case 3269:
        return new Item("Enchantment: Alacrity", "Enchants these boots to have the Alacrity bonus.\n\n\nUNIQUE Passive - Alacrity:\nYou gain +15 Movement Speed.");
    case 3270:
        return new Item("Enchantment: Homeguard", "Enchants these boots to have the Homeguard bonus.\n\n\nUNIQUE Passive - Homeguard:\nWhen you are at fountain and have been out of combat for 8 seconds you gain full health, mana and a 200% movement speed boost that decays over 8 seconds. You lose the movement speed when you enter combat.");
    case 3271:
        return new Item("Enchantment: Captain", "Enchants these boots to have the Captain bonus.\n\n\nUNIQUE Passive - Captain:\nAllied champions running towards you gain a 8% movement speed boost.  Nearby minions gain 25% additional movement speed.");
    case 3272:
        return new Item("Enchantment: Furor", "Enchants these boots to have the Furor bonus.\n\n\nUNIQUE Passive - Furor:\nWhenever you deal damage with a single target attack or spell, you gain 12% bonus movement speed that decays over 2 seconds.");
    case 3273:
        return new Item("Enchantment: Distortion", "Enchants these boots to have the Distortion bonus.\n\n\nUNIQUE Passive - Distortion:\nYour Teleport, Flash and Ghost summoner spell cooldowns are reduced by 25%.");
    case 3274:
        return new Item("Enchantment: Alacrity", "Enchants these boots to have the Alacrity bonus.\n\n\nUNIQUE Passive - Alacrity:\nYou gain +15 Movement Speed.");
    case 3275:
        return new Item("Enchantment: Homeguard", "Enchants these boots to have the Homeguard bonus.\n\n\nUNIQUE Passive - Homeguard:\nWhen you are at fountain and have been out of combat for 8 seconds you gain full health, mana and a 200% movement speed boost that decays over 8 seconds. You lose the movement speed when you enter combat.");
    case 3276:
        return new Item("Enchantment: Captain", "Enchants these boots to have the Captain bonus.\n\n\nUNIQUE Passive - Captain:\nAllied champions running towards you gain a 8% movement speed boost.  Nearby minions gain 25% additional movement speed.");
    case 3277:
        return new Item("Enchantment: Furor", "Enchants these boots to have the Furor bonus.\n\n\nUNIQUE Passive - Furor:\nWhenever you deal damage with a single target attack or spell, you gain 12% bonus movement speed that decays over 2 seconds.");
    case 3278:
        return new Item("Enchantment: Distortion", "Enchants these boots to have the Distortion bonus.\n\n\nUNIQUE Passive - Distortion:\nYour Teleport, Flash and Ghost summoner spell cooldowns are reduced by 25%.");
    case 3279:
        return new Item("Enchantment: Alacrity", "Enchants these boots to have the Alacrity bonus.\n\n\nUNIQUE Passive - Alacrity:\nYou gain +15 Movement Speed.");
    case 3280:
        return new Item("Enchantment: Homeguard", "Enchants these boots to have the Homeguard bonus.\n\n\nUNIQUE Passive - Homeguard:\nWhen you are at fountain and have been out of combat for 8 seconds you gain full health, mana and a 200% movement speed boost that decays over 8 seconds. You lose the movement speed when you enter combat.");
    case 3281:
        return new Item("Enchantment: Captain", "Enchants these boots to have the Captain bonus.\n\n\nUNIQUE Passive - Captain:\nAllied champions running towards you gain a 8% movement speed boost.  Nearby minions gain 25% additional movement speed.");
    case 3282:
        return new Item("Enchantment: Furor", "Enchants these boots to have the Furor bonus.\n\n\nUNIQUE Passive - Furor:\nWhenever you deal damage with a single target attack or spell, you gain 12% bonus movement speed that decays over 2 seconds.");
    case 3283:
        return new Item("Enchantment: Distortion", "Enchants these boots to have the Distortion bonus.\n\n\nUNIQUE Passive - Distortion:\nYour Teleport, Flash and Ghost summoner spell cooldowns are reduced by 25%.");
    case 3284:
        return new Item("Enchantment: Alacrity", "Enchants these boots to have the Alacrity bonus.\n\n\nUNIQUE Passive - Alacrity:\nYou gain +15 Movement Speed.");
    default:
        if(console.log)
            console.log('Unknown item id: ' + itemId);
        return new Item('Item ' + itemId, 'Unknown');
    }
}