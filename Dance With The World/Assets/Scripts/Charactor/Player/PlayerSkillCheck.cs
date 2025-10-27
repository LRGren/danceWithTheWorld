using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSkillCheck : MonoBehaviour
{
    public string objectName = "Player"; // 对象名字，用于成功时输出
    public string firstSequence = "w"; // 第一段序列（顺序敏感），可以只有一个字母
    public string secondSequence = "uijkl"; // 第二段序列（顺序不敏感）
    public float sequenceTimeout = 10f; // 整体超时时间
    public int stage = 1;
    public int priority = 0;
    public bool isOn = false;
    
    // 状态枚举
    private enum InputState
    {
        Idle, // 空闲状态，等待输入开始
        FirstSequence, // 输入第一段序列
        SecondSequence, // 输入第二段序列
        Success, // 输入成功
        Timeout // 输入超时
    }
    
    private InputState currentState = InputState.Idle;
    private int firstSequenceIndex = 0;
    private HashSet<char> secondSequenceRemaining;
    private HashSet<char> secondSequenceSet;
    public float stateTimer = 0f;
    
    // 事件：当输入成功时触发
    public event Action OnSuccess;
    
    void Update()
    {

        if (!GameManager.instance.isDancingMode)
        {
            currentState = InputState.Timeout;
        }
        
        switch (currentState)
        {
            case InputState.Idle:
                CheckForInputStart();
                break;
                
            case InputState.FirstSequence:
                ProcessFirstSequence();
                break;
                
            case InputState.SecondSequence:
                ProcessSecondSequence();
                break;
                
            case InputState.Success:
                // 成功状态，短暂延迟后重置
                StartCoroutine(ResetAfterDelay(0));
                currentState = InputState.Idle; // 立即回到空闲状态，避免重复执行
                break;
                
            case InputState.Timeout:
                // 超时状态，短暂延迟后重置
                StartCoroutine(ResetAfterDelay(0));
                currentState = InputState.Idle; // 立即回到空闲状态，避免重复执行
                break;
        }
    }
    
    /// <summary>
    /// 检查是否开始输入序列
    /// </summary>
    private void CheckForInputStart()
    {
        // 检测WASD按键，如果按下的是第一段序列的第一个字母，则开始输入序列
        char inputChar = GetWASDInput();
        if (inputChar != '\0' && inputChar == firstSequence[0])
        {
            // 如果第一段序列只有一个字母，检查是否需要第二段序列
            if (firstSequence.Length == 1)
            {
                // 如果第二段序列为空，直接成功
                if (string.IsNullOrEmpty(secondSequence))
                {
                    currentState = InputState.Success;
                    OutputSuccess();
                }
                else
                {
                    currentState = InputState.SecondSequence;
                    InitializeSecondSequence();
                    stateTimer = sequenceTimeout;
                }
            }
            else
            {
                currentState = InputState.FirstSequence;
                firstSequenceIndex = 1; // 第一个字母已经输入，从第二个开始
                stateTimer = sequenceTimeout;
            }
        }
    }
    
    /// <summary>
    /// 处理第一段序列输入
    /// </summary>
    private void ProcessFirstSequence()
    {
        stateTimer -= Time.deltaTime;
        
        // 检查超时
        if (stateTimer <= 0)
        {
            currentState = InputState.Timeout;
            return;
        }
        
        // 检查输入
        char inputChar = GetWASDInput();
        if (inputChar != '\0')
        {
            if (inputChar == firstSequence[firstSequenceIndex])
            {
                // 正确输入
                firstSequenceIndex++;
                stateTimer = sequenceTimeout; // 重置超时计时器
                
                // 第一段序列完成
                if (firstSequenceIndex >= firstSequence.Length)
                {
                    // 如果第二段序列为空，直接成功
                    if (string.IsNullOrEmpty(secondSequence))
                    {
                        currentState = InputState.Success;
                        OutputSuccess();  // 新增的直接成功逻辑
                    }
                    else
                    {
                        currentState = InputState.SecondSequence;
                        InitializeSecondSequence();
                        stateTimer = sequenceTimeout;
                    }
                }
            }
            else
            {
                // 输入错误，重置
                ResetInput();
                isOn = false;
            }
        }
    }
    
    /// <summary>
    /// 处理第二段序列输入
    /// </summary>
    private void ProcessSecondSequence()
    {
        stateTimer -= Time.deltaTime;
        
        // 检查超时
        if (stateTimer <= 0)
        {
            currentState = InputState.Timeout;
            return;
        }
        
        // 检查输入
        char inputChar = GetUIJKLInput();
        if (inputChar != '\0')
        {
            if (secondSequenceSet.Contains(inputChar))
            {
                if (secondSequenceRemaining.Contains(inputChar))
                {
                    // 正确输入
                    secondSequenceRemaining.Remove(inputChar);
                    stateTimer = sequenceTimeout; // 重置超时计时器
                    
                    // 第二段序列完成
                    if (secondSequenceRemaining.Count == 0)
                    {
                        currentState = InputState.Success;
                        OutputSuccess();  // 改为调用统一的输出方法
                    }
                }
                // 重复输入，忽略但不重置
            }
            else
            {
                // 输入了无效键，重置
                ResetInput();
                isOn = false;
            }
        }
    }
    
    /// <summary>
    /// 获取WASD输入
    /// </summary>
    private char GetWASDInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) return 'w';
        if (Input.GetKeyDown(KeyCode.A)) return 'a';
        if (Input.GetKeyDown(KeyCode.S)) return 's';
        if (Input.GetKeyDown(KeyCode.D)) return 'd';
        return '\0';
    }
    
    /// <summary>
    /// 获取UIJKL输入
    /// </summary>
    private char GetUIJKLInput()
    {
        if (Input.GetKeyDown(KeyCode.U)) return 'u';
        if (Input.GetKeyDown(KeyCode.I)) return 'i';
        if (Input.GetKeyDown(KeyCode.J)) return 'j';
        if (Input.GetKeyDown(KeyCode.K)) return 'k';
        if (Input.GetKeyDown(KeyCode.L)) return 'l';
        return '\0';
    }
    
    /// <summary>
    /// 初始化第二段序列
    /// </summary>
    private void InitializeSecondSequence()
    {
        secondSequenceSet = new HashSet<char>();
        secondSequenceRemaining = new HashSet<char>();
        
        foreach (char c in secondSequence)
        {
            secondSequenceSet.Add(c);
            secondSequenceRemaining.Add(c);
        }
    }
    
    /// <summary>
    /// 重置输入状态
    /// </summary>
    private void ResetInput()
    {
        currentState = InputState.Idle;
        firstSequenceIndex = 0;
        stateTimer = 0f;
    }
    
    /// <summary>
    /// 延迟后重置
    /// </summary>
    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetInput();
    }
    
    /// <summary>
    /// 执行成功后的操作
    /// </summary>
    private void PerformSuccessAction()
    {
        isOn = true;
        PlayerSkillInputManager.instance.TriggerMove(this);
    }
    
    /// <summary>
    /// 输出成功信息
    /// </summary>
    private void OutputSuccess()
    {
        // 输出成功信息
        //Debug.Log(objectName + " IS ON!!!");
    
        OnSuccess?.Invoke();
        PerformSuccessAction();
    }
}