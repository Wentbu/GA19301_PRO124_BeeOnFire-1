using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenerativeAI;
using GenerativeAI.Models;
using GenerativeAI.Types;
using System.Threading.Tasks;

public class GeminiManager : MonoBehaviour
{
    [SerializeField] private const string ApiKey = "AIzaSyBR30tZcgmCwnRjNBXYsM14jKRQiRK9Bg8";

    // Start is called before the first frame update
    void Start()
    {
        SetupGeminiAI();
    }

    private async void SetupGeminiAI()
    {
        var model = new GenerativeModel(ApiKey);
        //var model = new GeminiProModel(ApiKey);

        var chat = model.StartChat(new StartChatParams());
        var result = await chat.SendMessageAsync("Write a poem");

        Debug.Log("Initial Poem:\n" + result);
    }

}
