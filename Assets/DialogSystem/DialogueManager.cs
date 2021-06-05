using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

public class DialogueManager : MonoBehaviour
{
	public GameObject[] choiceButtons;
	public TextMeshProUGUI speakerName, dialogueText, navButtonText, relationText;
	public Image speakerSprite;
	public Speaker Wizard, Warrior, Rogue;

	private JsonData dialogue, currentLayer; //defines the current branche
	//****for the case of continuing the dialog after exiting the branches****
	private JsonData layerBeforeBranche;
	private int indexBeforeBranche;
	//************************************************************************
	private int index;        //defines the current position in the dialog
	private string textLine;
	private string question;
	private bool inDialogue;  //defines the end of the dialog


	public void loadDialogue()
	{
		index = 0;
		var jsonTextFile = Resources.Load<TextAsset>("DialogueText/JSON/DialogueText");
		dialogue = JsonMapper.ToObject(jsonTextFile.text);
		currentLayer = dialogue;
		inDialogue = true;
	}

	public void printLine()
	{
		if (inDialogue)
		{
			JsonData line = currentLayer[index];

			foreach (JsonData key in line.Keys)
				textLine = key.ToString();

			if (textLine == "EOD")  //End of dialog
			{
				inDialogue = false;
				dialogueText.text = "";
				return;
			}
			else if (textLine == "?")  //Branch
			{
				speakerName.text = "Wizard";
				speakerSprite.sprite = Wizard.GetSprite();

          			indexBeforeBranche = index;
          			layerBeforeBranche = currentLayer;
				dialogueText.text = question;
				JsonData options = line[0];

				for (int optionsNumber = 0; optionsNumber < options.Count; ++optionsNumber)
				{
					activateButton(choiceButtons[optionsNumber], options[optionsNumber]);
				}
			}
			else if (textLine == "RestoreIndex")  //Exit from branche
			{
				 index = indexBeforeBranche + 1;
          			 currentLayer = layerBeforeBranche;
				 printLine();
			}
			else  //Passage on a branch
			{
				speakerName.text = textLine;

				if (textLine == "Wizard")
				{
					speakerSprite.sprite = Wizard.GetSprite();
					relationText.text = "main character";
				}
				else if (textLine == "Warrior")
				{
					speakerSprite.sprite = Warrior.GetSprite();
					relationText.text = Warrior.GetRelation().ToString();
				}
				else
				{
					speakerSprite.sprite = Rogue.GetSprite();
					relationText.text = Rogue.GetRelation().ToString();
				}

				dialogueText.text = line[0].ToString();
				question = dialogueText.text;
				++index;
			}
		}
	}

	public void Start()
	{
		deactivateButtons();
		loadDialogue();
		speakerName.text = "";
		dialogueText.text = "";
		navButtonText.text = ">";
		Warrior.SetRelation(-Warrior.GetRelation());
		printLine();
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.N))
			printLine();
	}

	public void deactivateButtons()
	{
		foreach(GameObject button in choiceButtons)
		{
			button.SetActive(false);
			button.GetComponentInChildren<Text>().text = "";
			button.GetComponent<Button>().onClick.RemoveAllListeners();
		}
	}

	public void activateButton(GameObject button, JsonData choice)
	{
		button.SetActive(true);
		button.GetComponentInChildren<Text>().text = choice[0][0].ToString();
		button.GetComponent<Button>().onClick.AddListener(delegate { activeOnClick(choice); });
	}

	public void activeOnClick(JsonData choice)
	{
		currentLayer = choice[0];
		index = 1;
		string[] relCount = choice[0][0].ToString().Split(' ');  //parsing jsonData to extract the relation digit
		Warrior.SetRelation(int.Parse(relCount[0].ToString()));
		printLine();
		deactivateButtons();
	}
}
