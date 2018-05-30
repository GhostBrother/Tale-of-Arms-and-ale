﻿using UnityEngine;
using System.Collections;
using System;

public interface IBarManagerState  { 

    void ClickPatron();
    void MakeDrink(byte slotToUse, FillableMug targetMug ); 
    void PauseBar();
}



public class PatronHighlighted : IBarManagerState
{

    BarManager barManager;

    public PatronHighlighted(BarManager barsly)
    {
        barManager = barsly;
    }

    public void ClickPatron()
    {
        Debug.Log(barManager.SelectedSeat.patron.currentActivity);
        switch (barManager.SelectedSeat.patron.currentActivity)
        {
            case Patron.whatDoTheyWantToDo.TURNIN:
                {
                    barManager.OpenBattleReport();
                    barManager.SelectedSeat.patron.currentActivity = Patron.whatDoTheyWantToDo.GOHOME;
                    barManager.SelectedSeat.patron.QuestToCompleete = null;
                    barManager.setBarState(barManager.barIsPaused()); 
                    barManager.SelectedSeat.TalkWithPatron();
                    break;
                }

            case Patron.whatDoTheyWantToDo.ADVENTURE:
                {
                    if (barManager.SelectedSeat.patron.CurrentConversation == null)
                    {
                        barManager.SelectedSeat.patron.CurrentConversation = barManager.ConversationWarehouse.getRandomConversationBasedOnPatronID(barManager.SelectedSeat.patron.ID);
                        barManager.setBarState(barManager.patronIsConversing());
                        barManager.SelectedSeat.TalkWithPatron();
                    }
                    else
                    {
                        barManager.setBarState(barManager.patronPerformingAction());
                        barManager.SelectedSeat.PatronWantsToTalkAboutAdventure();
                    }

                    
                    break;
                }

            case Patron.whatDoTheyWantToDo.RUMOR:
                {
                    if (barManager.RumorManager.getNumberOfRumorsLeftInCharacter(barManager.SelectedSeat.patron.ID) == 0)
                    {
                       
                        barManager.SelectedSeat.patron.CurrentConversation = barManager.ConversationWarehouse.getRandomConversationBasedOnPatronID(barManager.SelectedSeat.patron.ID);
                        barManager.SelectedSeat.patron.currentActivity = Patron.whatDoTheyWantToDo.GOHOME;
                        barManager.setBarState(barManager.patronIsConversing());
                        barManager.SelectedSeat.TalkWithPatron();
                        break;
                    }
                    else
                    {
                        Rumor rumorToShare = barManager.RumorManager.getRandomRumorFromWarehouseByCharacter(barManager.SelectedSeat.patron.ID);
                        barManager.SelectedSeat.patron.CurrentConversation = barManager.ConversationWarehouse.getSpecificConversationFromLoader(barManager.SelectedSeat.patron.ID, rumorToShare.RumorName);
                        barManager.SendInfoToRumorBoard(rumorToShare);
                        barManager.setBarState(barManager.patronIsConversing());
                        barManager.SelectedSeat.TalkWithPatron();
                        break;
                    }
                }

            case Patron.whatDoTheyWantToDo.GOHOME:
                {
                    barManager.setBarState(barManager.dismissPatron());
                    break;
                }
            case Patron.whatDoTheyWantToDo.CONVERSE:
                {
                    barManager.setBarState(barManager.patronIsConversing());
                    barManager.SelectedSeat.TalkWithPatron();
                    break;
                }
        }
        
    }

    public void MakeDrink(byte SlotToUse, FillableMug targetMug)
    {
    }

    public void PauseBar()
    {
        barManager.setBarState(barManager.barIsPaused());
    }

}

public class PatronConversing : IBarManagerState
{ 
    BarManager barManager;

    public PatronConversing(BarManager barsly)
    {
        barManager = barsly;
    }

    public void ClickPatron()
    {
        if (barManager.SelectedSeat.patron.CurrentConversation == null)
        {
            barManager.setBarState(barManager.patronPerformingAction());
        }

        if (!barManager.SelectedSeat.patron.CurrentConversation.IsConversationOver)
        {
            barManager.SelectedSeat.TalkWithPatron();
        }
        else
        {
            barManager.UnlockContent(barManager.SelectedSeat.patron.CurrentConversation.ThingsThisConversationUnlocks);
            barManager.setBarState(barManager.prepareDrinkForPatron());
            barManager.ClickPatron(); 
        }
    }

    public void MakeDrink(byte SlotToUse, FillableMug targetMug)
    {

    }

    public void PauseBar()
    {
        barManager.setBarState(barManager.barIsPaused());
    }

}



public class MakePatronDrink : IBarManagerState
{
    BarManager barManager;

    public MakePatronDrink(BarManager barManager)
    {
        this.barManager = barManager;
    }

    public void ClickPatron()
    {
        if (barManager.SelectedSeat.CanDrink)
        {
            if (barManager.SelectedSeat.patron.OrderThePatronWants == null)
            {
                barManager.SelectedSeat.patron.OrderThePatronWants = barManager.OrderManager.makeARandomOrder();
                barManager.SelectedSeat.TalkWithPatron();
            }

            if (barManager.IsMugEmpty()) { barManager.theBarsTaps.UnlockTapSystem(); }
            
        }
        else
        {
            barManager.setBarState(barManager.patronPerformingAction());
            barManager.ClickPatron();     
        }
    }

    public void MakeDrink(byte slotToUse, FillableMug targetMug)
    {
        targetMug.AddIngredientToMug(barManager.theBarsTaps.useIngredent(slotToUse));
    }

    public void PauseBar()
    {
        throw new NotImplementedException();
    }
}

public class CompletePatronAction : IBarManagerState
{
    BarManager barManager;

    public CompletePatronAction(BarManager barsly)
    {
        barManager = barsly;
    }

    public void ClickPatron()
    {
        if (barManager.SelectedSeat.patron.currentActivity == Patron.whatDoTheyWantToDo.GOHOME)
        {
            barManager.SelectedSeat.TalkWithPatron();
            barManager.setBarState(barManager.dismissPatron());
        }

        if (barManager.SelectedSeat.patron.currentActivity == Patron.whatDoTheyWantToDo.RUMOR)
        {
            Debug.Log("Let's talk about quests!");
            barManager.OpenRumorBoard();
            barManager.setBarState(barManager.barIsPaused());
        }

        if (barManager.SelectedSeat.patron.currentActivity == Patron.whatDoTheyWantToDo.ADVENTURE)
        {
            Debug.Log("Let's go on an adventure");
            if (barManager.AreThereAnyActiveQuests())
            {
                barManager.SelectedSeat.IsPatronGoesOnQuestDeciderActive(true);
                //barManager.OpenMapFromBar(barManager.SelectedSeat.patron);
                barManager.setBarState(barManager.barIsPaused());
            }
            else
            {
                barManager.SelectedSeat.patron.currentActivity = Patron.whatDoTheyWantToDo.GOHOME;
                barManager.SelectedSeat.PatronTalksAboutNoMoreQuests(); // Added
                barManager.setBarState(barManager.dismissPatron());
            }
        }

        //if (barManager.SelectedSeat.patron.currentActivity == Patron.whatDoTheyWantToDo.GOHOME)
        //{
        //    barManager.SelectedSeat.TalkWithPatron();
        //    barManager.setBarState(barManager.dismissPatron());
        //}
    }

    public void MakeDrink(byte slotToUse, FillableMug TargetMug)
    {

    }

    public void PauseBar()
    {
        throw new NotImplementedException();
    }
}

public class DismissPatron : IBarManagerState
{
    BarManager barManager;

    public DismissPatron(BarManager barManager)
    {
        this.barManager = barManager;
    }

    public void ClickPatron()
    {
        Debug.Log("Patron prepares to leave has been called");
        if (barManager.SelectedSeat.patron.currentActivity == Patron.whatDoTheyWantToDo.RUMOR || barManager.SelectedSeat.patron.currentActivity == Patron.whatDoTheyWantToDo.GOHOME)
        {
          barManager.PatronManager.putAPatronBack(barManager.SelectedSeat.patron);      
        }

        if (barManager.SelectedSeat.patron.currentActivity == Patron.whatDoTheyWantToDo.ADVENTURE)
        {
            barManager.DropDownToast.AddMessageToQueue(barManager.SelectedSeat.patron.Name + " has left for a quest!");
        }

        barManager.SelectedSeat.patronPreparesToLeave();
        barManager.setBarState(barManager.noOneInteractedWith());
    }

    public void MakeDrink(byte slotToUse, FillableMug targetMug)
    {
        throw new NotImplementedException();
    }

    public void PauseBar()
    {
        throw new NotImplementedException();
    }
}

//// New Hack Class for the pause for the no adventure pause.
//public class NoAdventurePause : IBarManagerState
//{

//    BarManager barManager;


//    public NoAdventurePause(BarManager barsly)
//    {
//        barManager = barsly;

//    }

//    public void ClearPatronHighlight()
//    {

//    }

//    public void ClickPatron()
//    {
//        barManager.SelectedSeat.FadingText.cutOff(0);
//        //barManager.SelectedSeat.cutOffPatronsSentence();
//        barManager.setBarState(barManager.noOneInteractedWith());
//    }

//    public void MakeDrink(byte slotToUse, FillableMug targetMug)
//    {

//    }

//    public void PauseBar()
//    {

//    }

//}



public class BarPaused : IBarManagerState
{

    BarManager barManager;


    public BarPaused(BarManager barsly)
    {
        barManager = barsly;

    }

    public void ClearPatronHighlight()
    {

    }

    public void ClickPatron()
    {

    }

    public void MakeDrink(byte slotToUse, FillableMug targetMug)
    {

    }

    public void PauseBar()
    {

    }

}


    public class EndOfDayBar : IBarManagerState
    {

        BarManager barManager;


        public EndOfDayBar(BarManager barsly)
        {
            barManager = barsly;
        }

        public void ClearPatronHighlight()
        {

        }

        public void ClickPatron()
        {

        }

        public void MakeDrink(byte slotToUse, FillableMug targetMug)
        {

        }

        public void PauseBar()
        {

        }

    }






