using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour
{
    public string taskName;
    public string taskDescription;
    public bool completed;
    private TMP_Text _nameText;
    private TMP_Text _descriptionText;
    private GameObject _nameObject;
    private GameObject _descriptionObject;
    private TMP_InputField _nameInputField;
    private TMP_InputField _descriptionInputField;
    public GameObject container;
    public GameObject inputField;

    private void Start()
    {
        // 1. Setup Name Text and disable itself
        if (!_nameText)
        {
            _nameObject = Instantiate(container, this.transform);
            _nameObject.transform.position = new Vector3(0f, 0f, 0f);
            _nameText = _nameObject.GetComponentInChildren<TMP_Text>();
            _nameObject.GetComponent<Button>().onClick.AddListener(delegate { NameClicked(); });
            _nameObject.SetActive(false);
        }
        
        // 2. Setup Name Input Field and make OnEdit Trigger name change function
        if (!_nameInputField)
        {
            var obj = Instantiate(inputField, this.transform);
            _nameInputField = obj.GetComponent<TMP_InputField>();
            obj.transform.position = new Vector3(0f, 0f, 0f);
            obj.GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate { NameChange(); });
        }

        // 3. Setup Description Text and disable itself
        if (!_descriptionText)
        {
            _descriptionObject = Instantiate(container, this.transform);
            _descriptionObject.transform.position = new Vector3(0f, 0f, 0f);
            _descriptionText = _descriptionObject.GetComponentInChildren<TMP_Text>();
            _descriptionObject.GetComponent<Button>().onClick.AddListener(delegate { DescriptionClicked(); });
            _descriptionObject.SetActive(false);
        }

        // 4. Setup Description Input Field and make OnEdit Trigger Description Change function
        if (!_descriptionInputField)
        {
            var obj = Instantiate(inputField, this.transform);
            obj.transform.position = new Vector3(0f, 0f, 0f);
            _descriptionInputField = obj.GetComponent<TMP_InputField>();
            obj.GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate { DescriptionChange(); });
            obj.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 4);
        }
    }
    
    public void NameChange()
    {
        _nameText.text = _nameInputField.text;
        _nameObject.SetActive(true);
        _nameInputField.gameObject.SetActive(false);
    }
    // change the name text, disable the input field and enable the text upon edit

    public void DescriptionChange()
    {
        _descriptionText.text = _descriptionInputField.text;
        _descriptionObject.SetActive(true);
        _descriptionInputField.gameObject.SetActive(false); 
    }
    // change the description text, disable the input field and enable the text upon edit

    public void NameClicked()
    {
        _nameInputField.gameObject.SetActive(true);
        _nameObject.SetActive(false);
        _nameInputField.Select();
        _nameInputField.ActivateInputField();
    }

    public void DescriptionClicked()
    {
        _descriptionInputField.gameObject.SetActive(true); 
        _descriptionObject.SetActive(false);
        _descriptionInputField.Select();
        _descriptionInputField.ActivateInputField();
    }
}
