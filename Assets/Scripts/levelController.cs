using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class level
{
#if UNITY_EDITOR
    public string Name;
#endif
    public int levelNumber;
    public int tasksOnLevel;
    public int propsOnTaskCount;
    public List<prop> propsOnLevel;
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
    public GameObject taskStatusSpot;
    public GameObject taskStatusPrefab;
    public int currentLevelIndex;
    public levelsList levelsList;
    
    [Header("Debug")]
    public level _currentLevel;
    public List<GameObject> _tasksStatusObjects = new List<GameObject>();
    public int _taskCurrentNumber;
    public int _loseTasksOnLevel;
    public List<prop> _propsOnLevel;
    public List<prop> _propsOnTask = new List<prop>();
    public Sprite _rightSilhouette;
    public RuntimePlatform _platform;

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
        _taskCurrentNumber = 0;
        _loseTasksOnLevel = 0;
        shuffle(_propsOnLevel);
        generateTasksStatusOnLevel(_currentLevel.tasksOnLevel);
        generatePropsOnHiddenSpots(_propsOnLevel, _currentLevel.propsOnTaskCount);
        generatePropsCurrent();
    }
    
    private void generateTasksStatusOnLevel(int tasksOnLevel)
    {
        for (var i = 0; i < tasksOnLevel; i++)
        {
            var taskObject = Instantiate(taskStatusPrefab, taskStatusSpot.transform) as GameObject;
            _tasksStatusObjects.Add(taskObject);
        }
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

    private void generatePropsOnHiddenSpots(IReadOnlyList<prop> propsList, int propsOnTaskCount)
    {
        for (var i = 0; i < propsOnTaskCount; i++)
        {
            hiddenSpot[i].GetComponent<SpriteRenderer>().sprite = propsList[i].silhouette;
            hiddenSpot[i].tag = "Hidden prop";
            _propsOnTask.Add((prop)propsList[i]);
        }
    }


    private void generatePropsCurrent()
    {
        Debug.Log(_propsOnTask);
        var currentProp = _propsOnTask[Random.Range(0, _propsOnTask.Count - 1)];
        currentSpot.GetComponent<SpriteRenderer>().sprite = currentProp.drawProp;
        _rightSilhouette = currentProp.silhouette;
    }
        
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
                loseTask();
            }
        }
    }

    private void lose()
    {
        Debug.Log("lose");
        //loadLevel();
    }

    private void win()
    {
        Debug.Log("Win");
        winPanel.SetActive(true);
    }

    public void nextLevel()
    {
        if (_propsOnTask.Count > 0)
        {
            _propsOnTask.Clear();
        }

        if (_tasksStatusObjects.Count > 0)
        {
            _tasksStatusObjects.Clear();
        }
        
        foreach (Transform child in taskStatusSpot.transform) {
            GameObject.Destroy(child.gameObject);
        }
        currentLevelIndex++;
        _currentLevel = levelsList.getLevel(currentLevelIndex);
        loadLevel();
        winPanel.SetActive(false);
    }

    private void nextTask()
    {
        _propsOnTask.Clear();
        _propsOnLevel = _currentLevel.propsOnLevel;
        shuffle(_propsOnLevel);
        generatePropsOnHiddenSpots(_propsOnLevel, _currentLevel.propsOnTaskCount);
        generatePropsCurrent();
    }

    private void winTask()
    {
        _tasksStatusObjects[_taskCurrentNumber].GetComponent<Image>().color = Color.green;
        if (_taskCurrentNumber < _tasksStatusObjects.Count - 1)
        {
            nextTask();
            _taskCurrentNumber++;
        }
        else
        {
            win();
        }
    }

    private void loseTask()
    {
        _loseTasksOnLevel++;
        _tasksStatusObjects[_taskCurrentNumber].GetComponent<Image>().color = Color.red;
        if (_loseTasksOnLevel == 2)
        {
            lose();
        }
        else
        {
            if (_taskCurrentNumber < _tasksStatusObjects.Count - 1)
            {
                nextTask();
                _taskCurrentNumber++;
            }else
            {
                win();
            }
        }
    }

    private void nextProp()
    {
        if (_propsOnTask.Count > 1)
        {
            _propsOnTask = _propsOnTask.Where(prop => prop.silhouette != _rightSilhouette).ToList();
            generatePropsCurrent();
        }
        else
        {
            winTask();
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