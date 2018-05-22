﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class OrderManager : MonoBehaviour {

    private List<Drink> allLockedDrinksInGame = new List<Drink>();
    private DrinkLoader dLoader = new DrinkLoader();


    private List<Drink> allDrinksTheBartenderKnows = new List<Drink>();
    private List<Ingredient.ingredientColor> allUnlockedIngredentColors = new List<Ingredient.ingredientColor>();
    public enum OrderOptions {  BYCOLOR, BYWITHOUTCOLOR, LENGTH }

    public enum orderAccuracy {NONE,MIXUP, CORRECT};

    [SerializeField]
    Image[] allQuestionMarkBlocks = new Image[16]; // only problem with this is that I need to know order of drinks and load them in in proper order... I need to find a more dynamic way of doing this thing. 

    public void init()
    {
        dLoader.init();
        allLockedDrinksInGame = dLoader.populateDrinkCollection();
    }


    private Drink chooseRandomDrink()
    {
       int orderNumber = Random.Range(0, allDrinksTheBartenderKnows.Count);  //drinkCollection

        return allDrinksTheBartenderKnows[orderNumber];   //drinkCollection
    }

    public void unlockNewDrinksBasedOnIngredients(Ingredient.ingredientColor newIngredentColor)
    {
        Debug.Log("Unlocking color" + newIngredentColor.ToString());
        allUnlockedIngredentColors.Add(newIngredentColor);
        checkForNewDrinks();
    }

    private void checkForNewDrinks()
    {
        bool isDrinkAddable;
        bool isIngredentFound;
        for (int j = 0; j < allLockedDrinksInGame.Count; j++)
        {
            isDrinkAddable = true;
   
            for (Ingredient.ingredientColor i = 0; i < Ingredient.ingredientColor.LENGTH; i ++)
            {
                isIngredentFound = false;
                if (allLockedDrinksInGame[j].DrinkIngredents[(byte)i] == 0)
                {
                    continue;
                }
                
                for (int k = 0; k < allUnlockedIngredentColors.Count; k++)
                {
                    if (i == allUnlockedIngredentColors[k])
                    {
                        isIngredentFound = true;
                        break;
                    }

                }

                if (!isIngredentFound)
                {
                    isDrinkAddable = false;
                    break;
                }
               

            }
            if (isDrinkAddable)
            {
                allDrinksTheBartenderKnows.Add(allLockedDrinksInGame[j]);
                allLockedDrinksInGame.RemoveAt(j);
                j--;
            }
        }
    }

    public orderAccuracy determineDrinkPrice(IOrder PatronOrder , Drink drinkMade)
    {
        if (PatronOrder.checkAccuracy(drinkMade))
        { 
          
         SoundManager.Instance.AddCommand("Pay");
            return orderAccuracy.CORRECT;
        }
        else
        {
            return orderAccuracy.MIXUP;
        }
    }

    public Patron.SkillTypes GetDrinkBuffIfDrinkExistsAndIsUnlocked(Drink drinkTocheck)
    {

        for (int i = 0; i < allDrinksTheBartenderKnows.Count; i++)
        {

            // https://stackoverflow.com/questions/50098/comparing-two-collections-for-equality-irrespective-of-the-order-of-items-in-the
            // Note: if we want the order of ingredients to matter, we can remove the "OrderBy" part of the condition
            // NC: The order by was throwing off which buff was transfered, each drink returned the dragonbyte buff (Strong)\
            // Order also dosen't matter this way.
            if (allDrinksTheBartenderKnows[i].getIngredentsInDrink().SequenceEqual(drinkTocheck.getIngredentsInDrink()))
            {
                return allDrinksTheBartenderKnows[i].Buff;
            }
        }

        return Patron.SkillTypes.NONE;
    }

    public IOrder makeARandomOrder()
    {
        int randomNumber = Random.Range(0, (byte)OrderOptions.LENGTH);
        return (makeSpecificOrder((OrderOptions)randomNumber));
    }

    public IOrder makeSpecificOrder(OrderOptions requestedOrder)
    {
        
        switch (requestedOrder)
        {
            //case OrderOptions.BYNAME:
            //    {
            //        return new OrderByName(chooseRandomDrink());
                  
            //    }
            //case OrderOptions.BYFLAVOR:
            //    {
            //        return new OrderByFlavor(chooseRandomDrink().ThisDrinksFlavor);
                   
            //    }
            case OrderOptions.BYCOLOR:
                {
                    return new OrderByIngredent(chooseRandomIngredentFromKnownIngredents());
                 
                }

            case OrderOptions.BYWITHOUTCOLOR:
                {
                    return new OrderByLackOfIngredient();  
                }

            default:
                {
                    return new OrderByName(chooseRandomDrink());
                 
                }
        }
    }

    private Ingredient.ingredientColor chooseRandomIngredentFromKnownIngredents()
    {
        int randomNumber = Random.Range(0, allUnlockedIngredentColors.Count);
        return allUnlockedIngredentColors[randomNumber];
    }

    #region drinkAccessors
    
    public Drink getDrinkByName(string nameOfDrinkToFind)
    {
        foreach (Drink D in allDrinksTheBartenderKnows)
        {
            if (D.DrinkName == nameOfDrinkToFind)
            {
                return D;
            }
        }

        return null;
    }
    #endregion
}
