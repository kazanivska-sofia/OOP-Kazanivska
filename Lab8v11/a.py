hero = {
    "name": "Артур",
    "hp": 100,
    "mana": 40
}


def cast_spell(hero_dict, spell_name, mana_cost):
    if hero_dict["mana"] >= mana_cost:
        hero_dict["mana"] -= mana_cost
        print(f"{spell_name} успішно кастовано! Залишилося мани: {hero_dict['mana']}.")
    else:
        print(f"Недостатньо мани для заклинання {spell_name}!")


cast_spell(hero, "Fireball", 30)
cast_spell(hero, "Lightning", 20)
