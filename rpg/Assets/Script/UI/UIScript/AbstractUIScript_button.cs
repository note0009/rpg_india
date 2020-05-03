﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class ButtonData
{
    public string _buttonText;
    public UnityEvent _onClickAction = new UnityEvent();

    public ButtonData(string _text, UnityEvent _onclick)
    {
        _buttonText = _text;
        _onClickAction = _onclick;
    }
    public ButtonData(string _text)
    {
        _buttonText = _text;
    }
}
public abstract class AbstractUIScript_button : AbstractUIScript
{
    
    [SerializeField] Button _buttonPrefab;
    [SerializeField] int _buttonDisplayRange;
    [SerializeField,NonEditable] int _buttonRangeStartIndex;//表示範囲の始まり this=4 なら4～の_buttonDataListを表示
    [SerializeField,NonEditable] int _nowSelectButtonIndex;

    List<ButtonData> _buttonDataList = new List<ButtonData>();
    List<GameObject> _addButtonList = new List<GameObject>();
    RectTransform _myPanel { get { return _MyUIBase._myPanel; } }
    ButtonController _myButtonController { get { return _MyUIBase._myButtonController; } }

    protected abstract List<ButtonData> CreateMyButtonData();

    protected override void ChengeState_toActive()
    {
        base.ChengeState_toActive();
        var list = CreateMyButtonData();
        if (list != null)
        {
            //ボタンの追加
            ResetButtonData();
            AddButtonData(list);
            SyncButtonToText();
            SetSelectButton();
        }
    }

    protected override void ChengeState_toClose()
    {
        base.ChengeState_toClose();
        //追加されたボタンの削除
        ResetButtonData();
        SyncButtonToText();
        //indexの初期化
        _nowSelectButtonIndex = 0;
        _buttonRangeStartIndex = 0;
    }

    private void Update()
    {
        ButtonIndexUpdate();
    }
    //この下はprivate
    #region rawなボタン操作メソッド
    void AddButtonData(ButtonData data)
    {
        _buttonDataList.Add(data);
    }
    void AddButtonData(List<ButtonData> data)
    {
        _buttonDataList.AddRange(data);
    }
    void ResetButtonData()
    {
        _buttonDataList = new List<ButtonData>();
    }

    void SyncButtonToText()
    {
        ResetButton();
        for (int i = 0; i < _buttonDisplayRange; i++)
        {
            if (_buttonDataList.Count <= i + _buttonRangeStartIndex) break;
            var target = _buttonDataList[i + _buttonRangeStartIndex];
            var bt = AddButton(target._buttonText);
            bt.onClick.AddListener(() => target._onClickAction.Invoke());
        }
    }



    Button AddButton(string text)
    {
        var add = Instantiate(_buttonPrefab, _myPanel);
        var addText = add.GetComponentInChildren<Text>();
        addText.text = text;
        _addButtonList.Add(add.gameObject);
        return add;
    }

    void ResetButton()
    {
        foreach (var bt in _addButtonList)
        {
            Destroy(bt.gameObject);
        }
        _addButtonList = new List<GameObject>();
    }
    #endregion
    #region index関連

    void ButtonIndexUpdate()
    {
        _nowSelectButtonIndex = GetCurrentButtonIndex();
        if (_addButtonList.Count <= 0 || !_myButtonController._InputEnable) return;

        float tate = Input.GetAxisRaw("Vertical");
        if (tate == 0) return;
        bool isDown = tate < 0;

        if (CheckIndex_isRangeUpdateEnable(isDown)
            && CheckIndex_isUpdateRange(isDown))
        {
            if (isDown) _buttonRangeStartIndex++;
            else _buttonRangeStartIndex--;
            SyncButtonToText();
            SetSelectButton();
        }
    }

    int GetCurrentButtonIndex()
    {
        var select = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (_addButtonList.Contains(select))
        {
            return _addButtonList.IndexOf(select);
        }
        return _nowSelectButtonIndex;
    }
    
    void SetSelectButton()
    {
        //選択ボタンの設定
        if (_addButtonList.Count > 0)
        {
            if (_nowSelectButtonIndex < _addButtonList.Count)
            {
                _myButtonController.SetSelectButton(_addButtonList[_nowSelectButtonIndex].gameObject);
            }
            else
            {
                _myButtonController.SetSelectButton(_addButtonList[0].gameObject);
            }
        }
    }


    bool CheckIndex_isRangeUpdateEnable(bool down)
    {
        if (down) return _buttonRangeStartIndex < (_buttonDataList.Count - _buttonDisplayRange);
        else return _buttonRangeStartIndex > 0;
    }
    bool CheckIndex_isUpdateRange(bool down)
    {
        if (down) return _nowSelectButtonIndex + 1 == _buttonDisplayRange;
        else return _nowSelectButtonIndex == 0;
    }
    #endregion
}
