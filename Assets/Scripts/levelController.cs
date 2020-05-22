using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class level
{
#if UNITY_EDITOR
    public int levelNumber;
    public List<prop> propsOnLevel;
#endif
}

[System.Serializable]
public class prop
{
    public Sprite drawProp;
    public Sprite silhouette;
}

[System.Serializable]
public class levelsList
{
    public level[] levels;

    public level getLevel(int index)
    {
        return levels[index];
    }

    public int size()
    {
        return levels.Length;
    }
}

public class levelController : MonoBehaviour
{
    public GameObject[] hiddenSpot;
    public GameObject currentSpot;
    public GameObject levelLabel;
    public GameObject winPanel;
    public int currentLevelIndex;
    public levelsList levelsList;
    private level _currentLevel;
    private List<prop> _propsOnLevel;
    private Sprite _rightSilhouette;
    private RuntimePlatform _platform;

    // Start is called before the first frame update
    private void Start()
    {
        Input.simulateMouseWithTouches = true;
        checkPlatform();
        _currentLevel = levelsList.getLevel(currentLevelIndex);
        loadLevel();
    }

    private void checkPlatform()
    {
        _platform  = Application.platform;
    }

    private void loadLevel()
    {
        levelLabel.GetComponent<Text>().text = "Level " + _currentLevel.levelNumber;
        _propsOnLevel = _currentLevel.propsOnLevel;
        shuffle(_propsOnLevel);
        generatePropsOnHiddenSpots(_propsOnLevel);
        generatePropsCurrent();
    }

    private static void shuffle<T>(List<T> list)
    {
        for (var i = list.Count - 1; i >= 1; i--)
        {
            var j = Random.Range(0, i + 1);

            T tmp = list[j];
            list[j] = list[i];
            list[i] = tmp;
        }
    }

    private void generatePropsOnHiddenSpots(List<prop> propsList)
    {
        for (var i = 0; i < propsList.Count; i++)
        {
            hiddenSpot[i].GetComponent<SpriteRenderer>().sprite = propsList[i].silhouette;
            hiddenSpot[i].tag = "Hidden prop";
        }
    }


    private void generatePropsCurrent()
    {
        var currentProp = _propsOnLevel[Random.Range(0, _propsOnLevel.Count - 1)];
        currentSpot.GetComponent<SpriteRenderer>().sprite = currentProp.drawProp;
        _rightSilhouette = currentProp.silhouette;
    }

    //hit.transform.gameObject.SendMessage("OnMouseDown");
    //hit.transform.gameObject.SendMessage("Clicked",0,SendMessageOptions.DontRequireReceiver);
    private void Update()
    {
        if (_platform == RuntimePlatform.Android || _platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    checkTouch(Input.GetTouch(0).position);
                }
            }
        }
        else if (_platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                checkTouch(Input.mousePosition);
            }
        }
    }
    
    private void checkTouch(Vector3 position)
    {
        var wp  = Camera.main.ScreenToWorldPoint(position);
        var touchPosition = new Vector2(wp.x, wp.y);
        var hit = Physics2D.OverlapPoint(touchPosition);
        
        if (hit && hit.transform.gameObject.tag == "Hidden prop")
        {
            var selectedProp = hit.transform.gameObject;
            if (selectedProp.GetComponent<SpriteRenderer>().sprite == _rightSilhouette)
            {
                Debug.Log("true");
                changeHiddenToDraw(selectedProp);
                nextProp();
            }
            else
            {
                lose();
            }
        }
    }

    private void lose()
    {
        Debug.Log("lose");
        loadLevel();
    }

    private void win()
    {
        Debug.Log("Win");
        winPanel.SetActive(true);
    }

    private void nextProp()
    {
        if (_propsOnLevel.Count > 1)
        {
            _propsOnLevel = _propsOnLevel.Where(prop => prop.silhouette != _rightSilhouette).ToList();
            generatePropsCurrent();
        }
        else
        {
            win();
        }
    }

    private void changeHiddenToDraw(GameObject hiddenSpot)
    {
        hiddenSpot.tag = "Prop";
        hiddenSpot.GetComponent<SpriteRenderer>().sprite = currentSpot.GetComponent<SpriteRenderer>().sprite;
    }

    public int getLevelsCount()
    {
        return levelsList.size();
    }
    
}