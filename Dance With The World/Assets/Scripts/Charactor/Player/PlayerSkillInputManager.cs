using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SerializableDictionary
{
    [System.Serializable]
    public class KeyValuePair
    {
        public string key;
        public string[] value;

        public KeyValuePair(string key, string[] value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [SerializeField]
    private List<KeyValuePair> dictionary = new List<KeyValuePair>();

    // 索引器，方便通过key访问value
    public string[] this[string key]
    {
        get
        {
            var pair = dictionary.Find(x => x.key == key);
            return pair?.value;
        }
        set
        {
            var pair = dictionary.Find(x => x.key == key);
            if (pair != null)
            {
                pair.value = value;
            }
            else
            {
                dictionary.Add(new KeyValuePair(key, value));
            }
        }
    }

    // 添加键值对
    public void Add(string key, string[] value)
    {
        if (ContainsKey(key))
        {
            Debug.LogWarning($"Key '{key}' already exists in dictionary");
            return;
        }
        dictionary.Add(new KeyValuePair(key, value));
    }

    // 移除键值对
    public bool Remove(string key)
    {
        var pair = dictionary.Find(x => x.key == key);
        if (pair != null)
        {
            dictionary.Remove(pair);
            return true;
        }
        return false;
    }

    // 检查是否包含key
    public bool ContainsKey(string key)
    {
        return dictionary.Exists(x => x.key == key);
    }

    // 获取所有keys
    public List<string> Keys
    {
        get
        {
            var keys = new List<string>();
            foreach (var pair in dictionary)
            {
                keys.Add(pair.key);
            }
            return keys;
        }
    }

    // 获取所有values
    public List<string[]> Values
    {
        get
        {
            var values = new List<string[]>();
            foreach (var pair in dictionary)
            {
                values.Add(pair.value);
            }
            return values;
        }
    }

    // 清空字典
    public void Clear()
    {
        dictionary.Clear();
    }

    // 获取元素数量
    public int Count => dictionary.Count;

    // 转换为普通Dictionary
    public Dictionary<string, string[]> ToDictionary()
    {
        var result = new Dictionary<string, string[]>();
        foreach (var pair in dictionary)
        {
            result[pair.key] = pair.value;
        }
        return result;
    }
}

[System.Serializable]
public struct Skill
{
    public string SkillName;
    public string DirInput;
    public string BodyInput;
    public int Stage;
    public int Priority;
}

public class PlayerSkillInputManager : MonoBehaviour
{
    PlayerManager player;
    public static PlayerSkillInputManager instance;
    
    
    public bool enabledSkillInput = true;
    public string FINAL_SKILL_NAME;
    [SerializeField]private int CurrentStage = 1;
    [SerializeField]private PlayerSkillCheck current;
    
    public List<Skill> Skills;
    private Dictionary<string,PlayerSkillCheck> currentSkills =  new Dictionary<string, PlayerSkillCheck>();
    [SerializeField]
    private SerializableDictionary CanUpgrade = new SerializableDictionary();
    private string currentSkill = "null";
    
    [Header("Item")]
    private SkillItem[] items;
    
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for(int i = 0; i < Skills.Count; i++)
        {
            var child = new GameObject($"PSC_{i+1}");
            child.transform.SetParent(transform);
            
            var tmp = child.AddComponent<PlayerSkillCheck>();
            tmp.objectName = Skills[i].SkillName;
            tmp.firstSequence = Skills[i].DirInput;
            tmp.secondSequence = Skills[i].BodyInput;
            tmp.stage = Skills[i].Stage;
            tmp.priority= Skills[i].Priority;
            currentSkills.Add(Skills[i].SkillName, tmp);
        }
        
        if(!player)
            player = FindObjectOfType<PlayerManager>();
        
        DontDestroyOnLoad(this);
    }
    
    private void Update()
    {
        FINAL_SKILL_NAME = currentSkill;
    }

    int delayTime;
    public void Reset()
    {
        foreach (var pair in currentSkills)
        {
            pair.Value.stateTimer = 0f;
        }
    }
    
    public void TriggerMove(PlayerSkillCheck move)
    {
        if (move.stage == 1) 
        {
            if (current==null || current.priority <= move.priority)
            {
                player.playerAnimationManager.SetAnimBool("turnBack", false);
                
                delayTime = 900;
                current = move;
            }
        }
        else
        {
            //print(FINAL_SKILL_NAME + "  " + move.objectName + " " + MatchCorrect(FINAL_SKILL_NAME, move.objectName));
            
            if (!MatchCorrect(FINAL_SKILL_NAME, move.objectName))
                return;
            
            if (current == null || current.stage <= move.stage)
            {
                delayTime = 300;
                skillTrigger = true;
                current = move;
            }
        }
    }

    private bool MatchCorrect(string source, string target)
    {
        //Debug.Log(source + "   " + CanUpgrade.ContainsKey(source));
        
        if (CanUpgrade.ContainsKey(source))
        {
            string[] targetArray = CanUpgrade[source];
        
            if (targetArray != null)
            {
                //Debug.Log($"找到key '{source}'，数组长度: {targetArray.Length}");
                
                for (int i = 0; i < targetArray.Length; i++)
                {
                    //Debug.Log($"  [{i}] {targetArray[i]}");
                    if (target == targetArray[i])
                        return true;
                }
            }
        }

        return false;
    }
    
    bool skillTrigger = false;
    public void LateUpdate()
    {
        if (!enabledSkillInput)
        {
            CurrentStage = 0;
            current =  null;
        }
        
        delayTime--;
        if (delayTime <= 0)
        {
            player.playerAnimationManager.SetAnimBool("turnBack",true);
            
            CurrentStage = 0;
            current = null;
            currentSkill = "null";
            return;
        }
        
        if (current != null)
        {
            //print(current.objectName + "   " + FINAL_SKILL_NAME + "   " + MatchCorrect(FINAL_SKILL_NAME, current.objectName));

            if (current.stage == CurrentStage ||
                (CurrentStage == 3 && current.stage == 2) ||
                (current.stage == CurrentStage + 1 && MatchCorrect(FINAL_SKILL_NAME, current.objectName))) 
            {
                CurrentStage = current.stage;
                currentSkill = current.objectName;
            }
            
            if (skillTrigger)
            {
                //触发技能
                //print("Trigger Skill" + " " + FINAL_SKILL_NAME);
        
                /*if (FindObjectOfType<SkillItem>())
                {
                    items = FindObjectsOfType<SkillItem>();
                    //print(splineMovers.Length);
                }
                

                foreach (var item in items)
                {
                    print(item.MoverName + " " + currentSkill);
                    print(item.MoverName == currentSkill);
                        
                    if (item.MoverName == currentSkill)
                    {
                        item.trigger = true;
                    }
                }*/

                if (PlayerRaycastManager.instance.currentTarget != null)
                {
                    SkillItem item = PlayerRaycastManager.instance.currentTarget.transform.root
                        .GetComponentInChildren<SkillItem>();
                    
                    print(item.MoverName + " " + currentSkill);
                    
                    if (item.MoverName == currentSkill)
                        item.trigger = true;
                }
                    
                skillTrigger = false;
            }
        }
        
    }
}
