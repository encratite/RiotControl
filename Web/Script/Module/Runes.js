function Rune(name, description)
{
    this.name = name;
    this.description = description;
}

function getRune(id)
{
    switch(id)
    {
    case 5001:
        return new Rune("Lesser Mark of Strength", "+0.53 attack damage");
    case 5002:
        return new Rune("Lesser Mark of Might", "+0.08 attack damage per level (+1.35 at champion level 18)");
    case 5003:
        return new Rune("Lesser Mark of Alacrity", "+0.94% attack speed");
    case 5005:
        return new Rune("Lesser Mark of Furor", "+1.24% critical damage");
    case 5007:
        return new Rune("Lesser Mark of Malice", "+0.52% critical chance");
    case 5009:
        return new Rune("Lesser Mark of Desolation", "+0.93 armor penetration");
    case 5011:
        return new Rune("Lesser Mark of Fortitude", "+1.93 health");
    case 5012:
        return new Rune("Lesser Mark of Vitality", "+0.3 health per level (+5.4 at champion level 18)");
    case 5013:
        return new Rune("Lesser Mark of Resilience", "+0.51 armor");
    case 5015:
        return new Rune("Lesser Mark of Warding", "+0.54 magic resist");
    case 5016:
        return new Rune("Lesser Mark of Shielding", "+0.04 magic resist per level (+0.72 at champion level 18)");
    case 5021:
        return new Rune("Lesser Mark of Focus", "-0.09% cooldowns");
    case 5023:
        return new Rune("Lesser Mark of Potency", "+0.33 ability power");
    case 5024:
        return new Rune("Lesser Mark of Force", "+0.06 ability power per level (+1.08 at champion level 18)");
    case 5025:
        return new Rune("Lesser Mark of Intellect", "+3.28 mana");
    case 5026:
        return new Rune("Lesser Mark of Knowledge", "+0.65 mana per level (+11.7 at champion level 18)");
    case 5027:
        return new Rune("Lesser Mark of Replenishment", "+0.15 mana regen / 5 sec.");
    case 5029:
        return new Rune("Lesser Mark of Insight", "+0.53 magic penetration");
    case 5031:
        return new Rune("Lesser Glyph of Strength", "+0.16 attack damage");
    case 5032:
        return new Rune("Lesser Glyph of Might", "+0.02 attack damage per level (+0.36 at champion level 18)");
    case 5033:
        return new Rune("Lesser Glyph of Alacrity", "+0.35% attack speed");
    case 5035:
        return new Rune("Lesser Glyph of Furor", "+0.31% critical damage");
    case 5037:
        return new Rune("Lesser Glyph of Malice", "+0.15% critical chance");
    case 5041:
        return new Rune("Lesser Glyph of Fortitude", "+1.49 health");
    case 5042:
        return new Rune("Lesser Glyph of Vitality", "+0.3 health per level (+5.4 at champion level 18)");
    case 5043:
        return new Rune("Lesser Glyph of Resilience", "+0.39 armor");
    case 5045:
        return new Rune("Lesser Glyph of Warding", "+0.83 magic resist");
    case 5046:
        return new Rune("Lesser Glyph of Shielding", "+0.08 magic resist per level (+1.44 at champion level 18)");
    case 5047:
        return new Rune("Lesser Glyph of Vigor", "+0.15 health regen / 5 sec.");
    case 5051:
        return new Rune("Lesser Glyph of Focus", "-0.36% cooldowns");
    case 5052:
        return new Rune("Lesser Glyph of Celerity", "-0.03% cooldowns per level (-0.54% at champion level 18)");
    case 5053:
        return new Rune("Lesser Glyph of Potency", "+0.55 ability power");
    case 5054:
        return new Rune("Lesser Glyph of Force", "+0.1 ability power per level (+1.8 at champion level 18)");
    case 5055:
        return new Rune("Lesser Glyph of Intellect", "+6.25 mana");
    case 5056:
        return new Rune("Lesser Glyph of Knowledge", "+0.79 mana per level (+14.22 at champion level 18)");
    case 5057:
        return new Rune("Lesser Glyph of Replenishment", "+0.17 mana regen / 5 sec.");
    case 5058:
        return new Rune("Lesser Glyph of Clarity", "+0.03 mana regen / 5 sec. per level (+0.54 at champion level 18)");
    case 5059:
        return new Rune("Lesser Glyph of Insight", "+0.32 magic penetration");
    case 5061:
        return new Rune("Lesser Seal of Strength", "+0.24 attack damage");
    case 5062:
        return new Rune("Lesser Seal of Might", "+0.03 attack damage per level (+0.61 at champion level 18)");
    case 5063:
        return new Rune("Lesser Seal of Alacrity", "+0.42% attack speed");
    case 5065:
        return new Rune("Lesser Seal of Furor", "+0.43% critical damage");
    case 5067:
        return new Rune("Lesser Seal of Malice", "+0.23% critical chance");
    case 5071:
        return new Rune("Lesser Seal of Fortitude", "+2.97 health");
    case 5072:
        return new Rune("Lesser Seal of Vitality", "+0.6 health per level (+10.8 at champion level 18)");
    case 5073:
        return new Rune("Lesser Seal of Resilience", "+0.78 armor");
    case 5074:
        return new Rune("Lesser Seal of Defense", "+0.08 armor per level (+1.44 at champion level 18)");
    case 5075:
        return new Rune("Lesser Seal of Warding", "+0.41 magic resist");
    case 5076:
        return new Rune("Lesser Seal of Shielding", "+0.05 magic resist per level (+0.9 at champion level 18)");
    case 5077:
        return new Rune("Lesser Seal of Vigor", "+0.24 health regen / 5 sec.");
    case 5078:
        return new Rune("Lesser Seal of Regeneration", "+0.06 health regen / 5 sec. per level (+1.08 at champion level 18)");
    case 5081:
        return new Rune("Lesser Seal of Focus", "-0.16% cooldowns");
    case 5083:
        return new Rune("Lesser Seal of Potency", "+0.33 ability power");
    case 5084:
        return new Rune("Lesser Seal of Force", "+0.06 ability power per level (+1.08 at champion level 18)");
    case 5085:
        return new Rune("Lesser Seal of Intellect", "+3.83 mana");
    case 5086:
        return new Rune("Lesser Seal of Knowledge", "+0.65 mana per level (+11.7 at champion level 18)");
    case 5087:
        return new Rune("Lesser Seal of Replenishment", "+0.23 mana regen / 5 sec.");
    case 5088:
        return new Rune("Lesser Seal of Clarity", "+0.036 mana regen / 5 sec. per level (+0.65 at champion level 18)");
    case 5091:
        return new Rune("Lesser Quintessence of Strength", "+1.25 attack damage");
    case 5092:
        return new Rune("Lesser Quintessence of Might", "+0.14 attack damage per level (+2.52 at champion level 18)");
    case 5093:
        return new Rune("Lesser Quintessence of Alacrity", "+1.89% attack speed");
    case 5095:
        return new Rune("Lesser Quintessence of Furor", "+2.48% critical damage");
    case 5097:
        return new Rune("Lesser Quintessence of Malice", "+1.03% critical chance");
    case 5099:
        return new Rune("Lesser Quintessence of Desolation", "+1.85 armor penetration");
    case 5101:
        return new Rune("Lesser Quintessence of Fortitude", "+14.5 health");
    case 5102:
        return new Rune("Lesser Quintessence of Vitality", "+1.5 health per level (+27 at champion level 18)");
    case 5103:
        return new Rune("Lesser Quintessence of Resilience", "+2.37 armor");
    case 5104:
        return new Rune("Lesser Quintessence of Defense", "+0.21 armor per level (+3.78 at champion level 18)");
    case 5105:
        return new Rune("Lesser Quintessence of Warding", "+2.5 magic resist");
    case 5106:
        return new Rune("Lesser Quintessence of Shielding", "+0.21 magic resist per level (+3.78 at champion level 18)");
    case 5107:
        return new Rune("Lesser Quintessence of Vigor", "+1.5 health regen / 5 sec.");
    case 5108:
        return new Rune("Lesser Quintessence of Regeneration", "+0.16 health regen / 5 sec. per level (+2.88 at champion level 18)");
    case 5111:
        return new Rune("Lesser Quintessence of Focus", "-0.91% cooldowns");
    case 5112:
        return new Rune("Lesser Quintessence of Celerity", "-0.07% cooldowns per level (-1.26% at champion level 18)");
    case 5113:
        return new Rune("Lesser Quintessence of Potency", "+2.75 ability power");
    case 5114:
        return new Rune("Lesser Quintessence of Force", "+0.24 ability power per level (+4.32 at champion level 18)");
    case 5115:
        return new Rune("Lesser Quintessence of Intellect", "+20.83 mana");
    case 5116:
        return new Rune("Lesser Quintessence of Knowledge", "+2.31 mana per level (+41.58 at champion level 18)");
    case 5117:
        return new Rune("Lesser Quintessence of Replenishment", "+0.69 mana regen / 5 sec.");
    case 5118:
        return new Rune("Lesser Quintessence of Clarity", "+0.14 mana regen / 5 sec. per level (+2.52 at champion level 18)");
    case 5119:
        return new Rune("Lesser Quintessence of Insight", "+1.05 magic penetration");
    case 5121:
        return new Rune("Lesser Quintessence of Swiftness", "+0.83% movement speed");
    case 5123:
        return new Rune("Mark of Strength", "+0.74 attack damage");
    case 5124:
        return new Rune("Mark of Might", "+0.1 attack damage per level (+1.89 at champion level 18)");
    case 5125:
        return new Rune("Mark of Alacrity", "+1.32% attack speed");
    case 5127:
        return new Rune("Mark of Furor", "+1.74% critical damage");
    case 5129:
        return new Rune("Mark of Malice", "+0.72% critical chance");
    case 5131:
        return new Rune("Mark of Desolation", "+1.29 armor penetration");
    case 5133:
        return new Rune("Mark of Fortitude", "+2.7 health");
    case 5134:
        return new Rune("Mark of Vitality", "+0.42 health per level (+7.56 at champion level 18)");
    case 5135:
        return new Rune("Mark of Resilience", "+0.71 armor");
    case 5137:
        return new Rune("Mark of Warding", "+0.75 magic resist");
    case 5138:
        return new Rune("Mark of Shielding", "+0.06 magic resist per level (+1.08 at champion level 18)");
    case 5143:
        return new Rune("Mark of Focus", "-0.13% cooldowns");
    case 5145:
        return new Rune("Mark of Potency", "+0.46 ability power");
    case 5146:
        return new Rune("Mark of Force", "+0.08 ability power per level (+1.44 at champion level 18)");
    case 5147:
        return new Rune("Mark of Intellect", "+4.59 mana");
    case 5148:
        return new Rune("Mark of Knowledge", "+0.91 mana per level (+16.38 at champion level 18)");
    case 5149:
        return new Rune("Mark of Replenishment", "+0.2 mana regen / 5 sec.");
    case 5151:
        return new Rune("Mark of Insight", "+0.74 magic penetration");
    case 5153:
        return new Rune("Glyph of Strength", "+0.22 attack damage");
    case 5154:
        return new Rune("Glyph of Might", "+0.03 attack damage per level (+0.57 at champion level 18)");
    case 5155:
        return new Rune("Glyph of Alacrity", "+0.5% attack speed");
    case 5157:
        return new Rune("Glyph of Furor", "+0.43% critical damage");
    case 5159:
        return new Rune("Glyph of Malice", "+0.22% critical chance");
    case 5163:
        return new Rune("Glyph of Fortitude", "+2.08 health");
    case 5164:
        return new Rune("Glyph of Vitality", "+0.42 health per level (+7.56 at champion level 18)");
    case 5165:
        return new Rune("Glyph of Resilience", "+0.55 armor");
    case 5167:
        return new Rune("Glyph of Warding", "+1.16 magic resist");
    case 5168:
        return new Rune("Glyph of Shielding", "+0.12 magic resist per level (+2.16 at champion level 18)");
    case 5169:
        return new Rune("Glyph of Vigor", "+0.21 health regen / 5 sec.");
    case 5173:
        return new Rune("Glyph of Focus", "-0.51% cooldowns");
    case 5174:
        return new Rune("Glyph of Celerity", "-0.04% cooldowns per level (-0.72% at champion level 18)");
    case 5175:
        return new Rune("Glyph of Potency", "+0.77 ability power");
    case 5176:
        return new Rune("Glyph of Force", "+0.13 ability power per level (+2.34 at champion level 18)");
    case 5177:
        return new Rune("Glyph of Intellect", "+8.75 mana");
    case 5178:
        return new Rune("Glyph of Knowledge", "+1.1 mana per level (+19.8 at champion level 18)");
    case 5179:
        return new Rune("Glyph of Replenishment", "+0.24 mana regen / 5 sec.");
    case 5180:
        return new Rune("Glyph of Clarity", "+0.04 mana regen / 5 sec. per level (+0.72 at champion level 18)");
    case 5181:
        return new Rune("Glyph of Insight", "+0.44 magic penetration");
    case 5183:
        return new Rune("Seal of Strength", "+0.33 attack damage");
    case 5184:
        return new Rune("Seal of Might", "+0.05 attack damage per level (+0.85 at champion level 18)");
    case 5185:
        return new Rune("Seal of Alacrity", "+0.59% attack speed");
    case 5187:
        return new Rune("Seal of Furor", "+0.61% critical damage");
    case 5189:
        return new Rune("Seal of Malice", "+0.32% critical chance");
    case 5193:
        return new Rune("Seal of Fortitude", "+4.16 health");
    case 5194:
        return new Rune("Seal of Vitality", "+0.84 health per level (+15.12 at champion level 18)");
    case 5195:
        return new Rune("Seal of Resilience", "+1.09 armor");
    case 5196:
        return new Rune("Seal of Defense", "+0.12 armor per level (+2.16 at champion level 18)");
    case 5197:
        return new Rune("Seal of Warding", "+0.58 magic resist");
    case 5198:
        return new Rune("Seal of Shielding", "+0.08 magic resist per level (+1.44 at champion level 18)");
    case 5199:
        return new Rune("Seal of Vigor", "+0.34 health regen / 5 sec.");
    case 5200:
        return new Rune("Seal of Regeneration", "+0.09 health regen / 5 sec. per level (+1.62 at champion level 18)");
    case 5203:
        return new Rune("Seal of Focus", "-0.23% cooldowns");
    case 5205:
        return new Rune("Seal of Potency", "+0.46 ability power");
    case 5206:
        return new Rune("Seal of Force", "+0.08 ability power per level (+1.44 at champion level 18)");
    case 5207:
        return new Rune("Seal of Intellect", "+5.36 mana");
    case 5208:
        return new Rune("Seal of Knowledge", "+0.91 mana per level (+16.38 at champion level 18)");
    case 5209:
        return new Rune("Seal of Replenishment", "+0.32 mana regen / 5 sec.");
    case 5210:
        return new Rune("Seal of Clarity", "+0.05 mana regen / 5 sec. per level (+0.9 at champion level 18)");
    case 5213:
        return new Rune("Quintessence of Strength", "+1.75 attack damage");
    case 5214:
        return new Rune("Quintessence of Might", "+0.19 attack damage per level (+3.42 at champion level 18)");
    case 5215:
        return new Rune("Quintessence of Alacrity", "+2.64% attack speed");
    case 5217:
        return new Rune("Quintessence of Furor", "+3.47% critical damage");
    case 5219:
        return new Rune("Quintessence of Malice", "+1.44% critical chance");
    case 5221:
        return new Rune("Quintessence of Desolation", "+2.59 armor penetration");
    case 5223:
        return new Rune("Quintessence of Fortitude", "+20 health");
    case 5224:
        return new Rune("Quintessence of Vitality", "+2.1 health per level (+37.8 at champion level 18)");
    case 5225:
        return new Rune("Quintessence of Resilience", "+3.32 armor");
    case 5226:
        return new Rune("Quintessence of Defense", "+0.29 armor per level (+5.22 at champion level 18)");
    case 5227:
        return new Rune("Quintessence of Warding", "+3.5 magic resist");
    case 5228:
        return new Rune("Quintessence of Shielding", "+0.29 magic resist per level (+5.22 at champion level 18)");
    case 5229:
        return new Rune("Quintessence of Vigor", "+2.1 health regen / 5 sec.");
    case 5230:
        return new Rune("Quintessence of Regeneration", "+0.22 health regen / 5 sec. per level (+3.96 at champion level 18)");
    case 5233:
        return new Rune("Quintessence of Focus", "-1.27% cooldowns");
    case 5234:
        return new Rune("Quintessence of Celerity", "-0.1% cooldowns per level (-1.8% at champion level 18)");
    case 5235:
        return new Rune("Quintessence of Potency", "+3.85 ability power");
    case 5236:
        return new Rune("Quintessence of Force", "+0.34 ability power per level (+6.12 at champion level 18)");
    case 5237:
        return new Rune("Quintessence of Intellect", "+29.17 mana");
    case 5238:
        return new Rune("Quintessence of Knowledge", "+3.24 mana per level (+58.32 at champion level 18)");
    case 5239:
        return new Rune("Quintessence of Replenishment", "+0.97 mana regen / 5 sec.");
    case 5240:
        return new Rune("Quintessence of Clarity", "+0.19 mana regen / 5 sec. per level (+3.42 at champion level 18)");
    case 5241:
        return new Rune("Quintessence of Insight", "+1.47 magic penetration");
    case 5243:
        return new Rune("Quintessence of Swiftness", "+1.17% movement speed");
    case 5245:
        return new Rune("Greater Mark of Strength", "+0.95 attack damage");
    case 5246:
        return new Rune("Greater Mark of Might", "+0.13 attack damage per level (+2.43 at champion level 18)");
    case 5247:
        return new Rune("Greater Mark of Alacrity", "+1.7% attack speed");
    case 5249:
        return new Rune("Greater Mark of Furor", "+2.23% critical damage");
    case 5251:
        return new Rune("Greater Mark of Malice", "+0.93% critical chance");
    case 5253:
        return new Rune("Greater Mark of Desolation", "+1.66 armor penetration");
    case 5255:
        return new Rune("Greater Mark of Fortitude", "+3.47 health");
    case 5256:
        return new Rune("Greater Mark of Vitality", "+0.54 health per level (+9.72 at champion level 18)");
    case 5257:
        return new Rune("Greater Mark of Resilience", "+0.91 armor");
    case 5259:
        return new Rune("Greater Mark of Warding", "+0.97 magic resist");
    case 5260:
        return new Rune("Greater Mark of Shielding", "+0.07 magic resist per level (+1.26 at champion level 18)");
    case 5265:
        return new Rune("Greater Mark of Focus", "-0.16% cooldowns");
    case 5267:
        return new Rune("Greater Mark of Potency", "+0.59 ability power");
    case 5268:
        return new Rune("Greater Mark of Force", "+0.1 ability power per level (+1.8 at champion level 18)");
    case 5269:
        return new Rune("Greater Mark of Intellect", "+5.91 mana");
    case 5270:
        return new Rune("Greater Mark of Knowledge", "+1.17 mana per level (+21.06 at champion level 18)");
    case 5271:
        return new Rune("Greater Mark of Replenishment", "+0.26 mana regen / 5 sec.");
    case 5273:
        return new Rune("Greater Mark of Insight", "+0.95 magic penetration");
    case 5275:
        return new Rune("Greater Glyph of Strength", "+0.28 attack damage");
    case 5276:
        return new Rune("Greater Glyph of Might", "+0.04 attack damage per level (+0.73 at champion level 18)");
    case 5277:
        return new Rune("Greater Glyph of Alacrity", "+0.64% attack speed");
    case 5279:
        return new Rune("Greater Glyph of Furor", "+0.56% critical damage");
    case 5281:
        return new Rune("Greater Glyph of Malice", "+0.28% critical chance");
    case 5285:
        return new Rune("Greater Glyph of Fortitude", "+2.67 health");
    case 5286:
        return new Rune("Greater Glyph of Vitality", "+0.54 health per level (+9.72 at champion level 18)");
    case 5287:
        return new Rune("Greater Glyph of Resilience", "+0.7 armor");
    case 5289:
        return new Rune("Greater Glyph of Warding", "+1.49 magic resist");
    case 5290:
        return new Rune("Greater Glyph of Shielding", "+0.15 magic resist per level (+2.7 at champion level 18)");
    case 5291:
        return new Rune("Greater Glyph of Vigor", "+0.27 health regen / 5 sec.");
    case 5295:
        return new Rune("Greater Glyph of Focus", "-0.65% cooldowns");
    case 5296:
        return new Rune("Greater Glyph of Celerity", "-0.05% cooldowns per level (-0.9% at champion level 18)");
    case 5297:
        return new Rune("Greater Glyph of Potency", "+0.99 ability power");
    case 5298:
        return new Rune("Greater Glyph of Force", "+0.17 ability power per level (+3.06 at champion level 18)");
    case 5299:
        return new Rune("Greater Glyph of Intellect", "+11.25 mana");
    case 5300:
        return new Rune("Greater Glyph of Knowledge", "+1.42 mana per level (+25.56 at champion level 18)");
    case 5301:
        return new Rune("Greater Glyph of Replenishment", "+0.31 mana regen / 5 sec.");
    case 5302:
        return new Rune("Greater Glyph of Clarity", "+0.055 mana regen / 5 sec. per level (+0.99 at champion level 18)");
    case 5303:
        return new Rune("Greater Glyph of Insight", "+0.57 magic penetration");
    case 5305:
        return new Rune("Greater Seal of Strength", "+0.43 attack damage");
    case 5306:
        return new Rune("Greater Seal of Might", "+0.06 attack damage per level (+1.09 at champion level 18)");
    case 5307:
        return new Rune("Greater Seal of Alacrity", "+0.76% attack speed");
    case 5309:
        return new Rune("Greater Seal of Furor", "+0.78% critical damage");
    case 5311:
        return new Rune("Greater Seal of Malice", "+0.42% critical chance");
    case 5315:
        return new Rune("Greater Seal of Fortitude", "+5.35 health");
    case 5316:
        return new Rune("Greater Seal of Vitality", "+1.08 health per level (+19.44 at champion level 18)");
    case 5317:
        return new Rune("Greater Seal of Resilience", "+1.41 armor");
    case 5318:
        return new Rune("Greater Seal of Defense", "+0.15 armor per level (+2.7 at champion level 18)");
    case 5319:
        return new Rune("Greater Seal of Warding", "+0.74 magic resist");
    case 5320:
        return new Rune("Greater Seal of Shielding", "+0.1 magic resist per level (+1.8 at champion level 18)");
    case 5321:
        return new Rune("Greater Seal of Vigor", "+0.43 health regen / 5 sec.");
    case 5322:
        return new Rune("Greater Seal of Regeneration", "+0.11 health regen / 5 sec. per level (+1.98 at champion level 18)");
    case 5325:
        return new Rune("Greater Seal of Focus", "-0.29% cooldowns");
    case 5327:
        return new Rune("Greater Seal of Potency", "+0.59 ability power");
    case 5328:
        return new Rune("Greater Seal of Force", "+0.1 ability power per level (+1.8 at champion level 18)");
    case 5329:
        return new Rune("Greater Seal of Intellect", "+6.89 mana");
    case 5330:
        return new Rune("Greater Seal of Knowledge", "+1.17 mana per level (+21.06 at champion level 18)");
    case 5331:
        return new Rune("Greater Seal of Replenishment", "+0.41 mana regen / 5 sec.");
    case 5332:
        return new Rune("Greater Seal of Clarity", "+0.065 mana regen / 5 sec. per level (+1.17 at champion level 18)");
    case 5335:
        return new Rune("Greater Quintessence of Strength", "+2.25 attack damage");
    case 5336:
        return new Rune("Greater Quintessence of Might", "+0.25 attack damage per level (+4.5 at champion level 18)");
    case 5337:
        return new Rune("Greater Quintessence of Alacrity", "+3.4% attack speed");
    case 5339:
        return new Rune("Greater Quintessence of Furor", "+4.46% critical damage");
    case 5341:
        return new Rune("Greater Quintessence of Malice", "+1.86% critical chance");
    case 5343:
        return new Rune("Greater Quintessence of Desolation", "+3.33 armor penetration");
    case 5345:
        return new Rune("Greater Quintessence of Fortitude", "+26 health");
    case 5346:
        return new Rune("Greater Quintessence of Vitality", "+2.7 health per level (+48.6 at champion level 18)");
    case 5347:
        return new Rune("Greater Quintessence of Resilience", "+4.26 armor");
    case 5348:
        return new Rune("Greater Quintessence of Defense", "+0.38 armor per level (+6.84 at champion level 18)");
    case 5349:
        return new Rune("Greater Quintessence of Warding", "+4.5 magic resist");
    case 5350:
        return new Rune("Greater Quintessence of Shielding", "+0.37 magic resist per level (+6.66 at champion level 18)");
    case 5351:
        return new Rune("Greater Quintessence of Vigor", "+2.7 health regen / 5 sec.");
    case 5352:
        return new Rune("Greater Quintessence of Regeneration", "+0.28 health regen / 5 sec. per level (+5.04 at champion level 18)");
    case 5355:
        return new Rune("Greater Quintessence of Focus", "-1.64% cooldowns");
    case 5356:
        return new Rune("Greater Quintessence of Celerity", "-0.13% cooldowns per level (-2.34% at champion level 18)");
    case 5357:
        return new Rune("Greater Quintessence of Potency", "+4.95 ability power");
    case 5358:
        return new Rune("Greater Quintessence of Force", "+0.43 ability power per level (+7.74 at champion level 18)");
    case 5359:
        return new Rune("Greater Quintessence of Intellect", "+37.5 mana");
    case 5360:
        return new Rune("Greater Quintessence of Knowledge", "+4.17 mana per level (+75.06 at champion level 18)");
    case 5361:
        return new Rune("Greater Quintessence of Replenishment", "+1.25 mana regen / 5 sec.");
    case 5362:
        return new Rune("Greater Quintessence of Clarity", "+0.24 mana regen / 5 sec. per level (+4.32 at champion level 18)");
    case 5363:
        return new Rune("Greater Quintessence of Insight", "+1.89 magic penetration");
    case 5365:
        return new Rune("Greater Quintessence of Swiftness", "+1.5% movement speed");
    case 5366:
        return new Rune("Greater Quintessence of Revival", "-5% time dead");
    case 5367:
        return new Rune("Greater Quintessence of Avarice", "+1 gold / 10 sec.");
    case 5368:
        return new Rune("Greater Quintessence of Wisdom", "+2% experience gained");
    case 5369:
        return new Rune("Greater Seal of Meditation", "+0.63 Energy regen/5 sec");
    case 5370:
        return new Rune("Greater Seal of Lucidity", "+0.064 Energy regen/5 sec per level (+1.15 at champion level 18)");
    case 5371:
        return new Rune("Greater Glyph of Acumen", "+2.2 Energy");
    case 5372:
        return new Rune("Greater Glyph of Sapience", "+0.161 Energy/level (+2.89 at level 18)");
    case 5373:
        return new Rune("Greater Quintessence of Meditation", "+1.575 Energy regen/5 sec");
    case 5374:
        return new Rune("Greater Quintessence of Acumen", "+5.4 Energy");
    case 5400:
        return new Rune("Lesser Mark of Destruction", "+.56 Armor Penetration / +.32 Magic Penetration");
    case 5401:
        return new Rune("Mark of Destruction", "+.74 Armor Penetration / +.44 Magic Penetration");
    case 5402:
        return new Rune("Greater Mark of Destruction", "+1.0 Armor Penetration / +.57 Magic Penetration");
    case 5403:
        return new Rune("Greater Seal of Avarice", "+0.25 gold / 10 sec.");
    case 5404:
        return new Rune("Lesser Quintessence of Endurance", "+0.84% increased health.");
    case 5405:
        return new Rune("Quintessence of Endurance", "+1.17% increased health.");
    case 5406:
        return new Rune("Greater Quintessence of Endurance", "+1.5% increased health.");
    case 5407:
        return new Rune("Lesser Quintessence of Transmutation", "+1.12% Spellvamp.");
    case 5408:
        return new Rune("Quintessence of Transmutation", "+1.56% Spellvamp.");
    case 5409:
        return new Rune("Greater Quintessence of Transmutation", "+2% Spellvamp.");
    case 5410:
        return new Rune("Lesser Quintessence of Vampirism", "+1.12% Lifesteal");
    case 5411:
        return new Rune("Quintessence of Vampirism", "+1.56% Lifesteal");
    case 5412:
        return new Rune("Greater Quintessence of Vampirism", "+2% Lifesteal.");
    case 5413:
        return new Rune("Lesser Seal of Endurance", "+0.28% Health.");
    case 5414:
        return new Rune("Seal of Endurance", "+0.39% Health.");
    case 5415:
        return new Rune("Greater Seal of Endurance", "+0.5% Health.");
    case 5416:
        return new Rune("Lesser Quintessence of Destruction", "+1.11 Armor Penetration / +0.63 Magic Penetration");
    case 5417:
        return new Rune("Quintessence of Destruction", "+1.55 Armor Penetration / +0.88 Magic Penetration");
    case 5418:
        return new Rune("Greater Quintessence of Destruction", "+2.0 Armor Penetration / +1.13 Magic Penetration");
    case 8001:
        return new Rune("Mark of the Crippling Candy Cane", "+2% critical damage");
    case 8002:
        return new Rune("Lesser Mark of the Yuletide Tannenbaum ", "+0.62% critical chance");
    case 8003:
        return new Rune("Glyph of the Special Stocking", "-0.58% cooldowns");
    case 8005:
        return new Rune("Lesser Glyph of the Gracious Gift", "+0.12 ability power per level (+2.16 at champion level 18)");
    case 8006:
        return new Rune("Lesser Seal of the Stout Snowman", "+0.72 health per level (+12.96 at champion level 18)");
    case 8007:
        return new Rune("Lesser Mark of Alpine Alacrity", "+1.13% attack speed");
    case 8008:
        return new Rune("Mark of the Combatant", "+2% critical damage");
    case 8009:
        return new Rune("Lesser Seal of the Medalist", "+3.56 health");
    case 8011:
        return new Rune("Lesser Glyph of the Challenger", "+0.66 ability power");
    case 8012:
        return new Rune("Glyph of the Soaring Slalom", "-0.58% cooldowns");
    case 8013:
        return new Rune("Quintessence of the Headless Horseman", "+3.08 armor penetration");
    case 8014:
        return new Rune("Quintessence of the Piercing Screech", "+1.75 magic penetration");
    case 8015:
        return new Rune("Quintessence of Bountiful Treats", "+24 health");
    case 8016:
        return new Rune("Quintessence of the Speedy Specter", "+1.39% movement speed");
    case 8017:
        return new Rune("Quintessence of the Witches Brew", "+4.56 ability power");
    case 8019:
        return new Rune("Greater Quintessence of the Piercing Present", "+1.89 magic penetration");
    case 8020:
        return new Rune("Greater Quintessence of the Deadly Wreath", "+3.33 armor penetration");
    case 8021:
        return new Rune("Greater Quintessence of Frosty Fortitude", "+26 health");
    case 8022:
        return new Rune("Greater Quintessence of Sugar Rush", "+1.5% movement speed");
    case 8035:
        return new Rune("Greater Quintessence of Studio Rumble", "+1.5% movement speed");
    case 10001:
        return new Rune("Razer Mark of Precision", "+2.23% critical damage");
    case 10002:
        return new Rune("Razer Quintessence of Speed", "+1.5% movement speed");
    default:
        return new Rune('Unknown rune', 'Unknown');
    }
}