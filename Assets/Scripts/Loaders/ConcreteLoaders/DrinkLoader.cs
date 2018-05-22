﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DrinkLoader : Loader  {

    private string FallThroughHelper;  // used to help me find JSON typos
                                       // private static patronLoader instance = null;
                                       // private static readonly object padloc = new object();

    private enum drinkjsonHelper {NAME,RED,YELLOW,BLUE,GREEN,FLAVOR,CORRECT,MIXUP,BUFF, DESCRIPTION};   

    public override void init()
    {
        loadJson("/JsonFiles/DrinkList.json");
    }

    public List<Drink> populateDrinkCollection()
    {
        List<Drink> drinkListToReturn = new List<Drink>();
        for (int i = 0; i < jsonObject.Count; i++)
        {
           drinkListToReturn.Add(DrinkCreator(i));
        }
        return drinkListToReturn;
    }



    private Drink DrinkCreator(int drinkIndexer)
    {
        Drink drinkToCreate = new Drink();
        drinkToCreate.DrinkName = jsonObject[drinkIndexer][(int)drinkjsonHelper.NAME].str; 
        drinkToCreate.DrinkIngredents[0] = (byte)jsonObject[drinkIndexer][(int)drinkjsonHelper.RED].i;
        drinkToCreate.DrinkIngredents[1] = (byte)jsonObject[drinkIndexer][(int)drinkjsonHelper.YELLOW].i;
        drinkToCreate.DrinkIngredents[2] = (byte)jsonObject[drinkIndexer][(int)drinkjsonHelper.GREEN].i;
        drinkToCreate.DrinkIngredents[3] = (byte)jsonObject[drinkIndexer][(int)drinkjsonHelper.BLUE].i;
        drinkToCreate.NumberOfIngredentsInDrink = addAllIngredents(drinkToCreate);
        drinkToCreate.Buff = drinkBuffParser(jsonObject[drinkIndexer][(int)drinkjsonHelper.BUFF].str);
        drinkToCreate.DrinkDescription = jsonObject[drinkIndexer][(int)drinkjsonHelper.DESCRIPTION].str;
        drinkToCreate.RecipeForDrink = createRecipe(drinkToCreate);

        return drinkToCreate;
    }

    private string createRecipe(Drink drinkToCreatRecipieFor)
    {
        string stringToReturn = string.Empty;
        Ingredient tempIngredent = new Ingredient(Ingredient.ingredientColor.LENGTH); // HACK used as an indexer Ingredent to help us get the names for the ingredents
        for (Ingredient.ingredientColor i = 0; i < Ingredient.ingredientColor.LENGTH; i++)
        {
            if (drinkToCreatRecipieFor.DrinkIngredents[(byte)i] > 0)
            {
                tempIngredent.ThisIngredentsColor = i;
                stringToReturn += drinkToCreatRecipieFor.DrinkIngredents[(byte)i] + "X" + tempIngredent.sayName().ToString().ToLower() + "\n";
            }
        }
        return stringToReturn;
    }

    private int addAllIngredents(Drink drinkWithIngredentsToCount)
    {
        int runningTotalToReturn = 0;

        for (int i = 0; i < drinkWithIngredentsToCount.DrinkIngredents.Length; i++)
        {
            runningTotalToReturn += drinkWithIngredentsToCount.DrinkIngredents[i];
        }

        return runningTotalToReturn;
    }

    private Patron.SkillTypes drinkBuffParser(string buffToParse)
    {
        switch (buffToParse.ToLower())
        {

            case "strong":
                {
                    return Patron.SkillTypes.STRONG;
                }
            case "smart":
                {
                    return Patron.SkillTypes.SMART;
                }
            case "sneak":
                {
                    return Patron.SkillTypes.SNEAK;
                }
            case "sway":
                {
                    return Patron.SkillTypes.SWAY;
                }
            case "none":
                {
                    return Patron.SkillTypes.NONE;
                }
            default:
                {
                    Debug.Log("Buff Fall through:" + FallThroughHelper);
                    // return Patron.SkillTypes.STRONG;
                    return Patron.SkillTypes.NONE;
                }

        }


    }
}
