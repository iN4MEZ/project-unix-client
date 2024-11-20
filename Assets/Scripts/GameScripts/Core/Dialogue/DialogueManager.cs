using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NMX
{
    public class DialogueManager : MonoBehaviour
    {
        public GameObject CanvasBox; // your fancy canvas box that holds your text objects
        public Text TextBox; // the text body
        public Text NameText; // the text body of the name you want to display
        public bool freezePlayerOnDialogue = true;

        // private bool isOpen; // represents if the dialogue box is open or closed

        private Queue<string> inputStream = new Queue<string>(); // stores dialogue

        private void Start()
        {
            CanvasBox = UIManager.instance.DialogueCanvasBox;
            TextBox = UIManager.instance.DialogueTextBox;
            NameText = UIManager.instance.DialogueNameText;

            CanvasBox.SetActive(false); // close the dialogue box on play

        }

        private void DisablePlayerController()
        {

        }

        public void StartDialogue(Queue<string> dialogue)
        {
            if (freezePlayerOnDialogue)
            {
                DisablePlayerController();
            }

            CanvasBox.SetActive(true); // open the dialogue box
                                       // isOpen = true;
            inputStream = dialogue; // store the dialogue from dialogue trigger
            PrintDialogue(); // Prints out the first line of dialogue
        }

        public void AdvanceDialogue() // call when a player presses a button in Dialogue Trigger
        {
            PrintDialogue();
        }

        private void PrintDialogue()
        {
            if (inputStream.Count == 0 || inputStream.Peek().Contains("EndQueue")) // special phrase to stop dialogue
            {
                inputStream.Dequeue(); // Clear Queue
                EndDialogue();
            }
            else if (inputStream.Peek().Contains("[NAME="))
            {
                string name = inputStream.Peek();
                name = inputStream.Dequeue().Substring(name.IndexOf('=') + 1, name.IndexOf(']') - (name.IndexOf('=') + 1));
                NameText.text = name;
                PrintDialogue(); // print the rest of this line
            }
            else
            {
                TextBox.text = inputStream.Dequeue();
            }
        }

        public void EndDialogue()
        {
            TextBox.text = "";
            NameText.text = "";
            inputStream.Clear();
            CanvasBox.SetActive(false);
            // isOpen = false;
        }
    }
}
