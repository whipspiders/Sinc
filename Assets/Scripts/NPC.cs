
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC")] 
public class NPC : ScriptableObject
{
    public Sprite npcSprite;
    public string npcName;

    public string[] hiNames =
    {
        "КОЛЛЕКТОР",
        "КОЛЛЕКТОР",
        "КОЛЛЕКТОР"
    };
    public string[] hiDialogues =
    {
        "Привет землянин. Вынуждена оповестить что ваша подписка на космическое пронстранство закончилась.",
        "И по закону о межгалактическом развитии цивилизаций планета Земля должна была перестать функционировать примерно две недели назад уступив место новым мирам и формам жизни. ",
        "This is the third line"
    };

    public string[] byeDialogues =
    {
        "Line after mini-game",
        "Second line after mini-game",
        "Third line after mini-game"
    };
}