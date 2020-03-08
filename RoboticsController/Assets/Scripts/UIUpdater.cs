using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

public class UIUpdater : MonoBehaviour
{
    BotEmulator emulator;

    public Button[] keyButtons;
    public string[] alternates;
    [SerializeField] string[] defaults;
    Brick brick;

    public bool Fwd_FLAG = false;
    public bool Bwd_FLAG = false;
    public bool Lft_FLAG = false;
    public bool Rgt_FLAG = false;

    public enum Buttons : int
    {
        W,
        A,
        S,
        D,
        Space,
        F1
    }

    async void Awake()
    {
        emulator = FindObjectOfType<BotEmulator>();
        defaults = new string[alternates.Length];
        for (int i = 0; i < alternates.Length; i++)
        {
            defaults[i] = keyButtons[i].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text;
        }
        await ToggleHelpDropdown(true);
        Main();
    }

    async static void Main()
    {
        Brick _brick = FindObjectOfType<UIUpdater>().brick;
        _brick = new Brick(new UsbCommunication(), true);
        await _brick.ConnectAsync();
        _brick.BrickChanged += FindObjectOfType<UIUpdater>().Brick_BrickChanged;
    }

    bool dropdownDown = true;
    async void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            for (int i = 0; i < alternates.Length; i++)
            {
                keyButtons[i].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = alternates[i];
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            for (int i = 0; i < defaults.Length; i++)
            {
                keyButtons[i].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = defaults[i];
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            dropdownDown = !dropdownDown;
            await ToggleHelpDropdown(dropdownDown);
        }

        #region FwdBwd
        if (Input.GetKeyDown(KeyCode.W))
        {
            Fwd_FLAG = true;
            Bwd_FLAG = false;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            Fwd_FLAG = false;
            if (Input.GetKey(KeyCode.S))
                Bwd_FLAG = true;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Fwd_FLAG = false;
            Bwd_FLAG = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Bwd_FLAG = false;
            if (Input.GetKey(KeyCode.W))
                Fwd_FLAG = true;
        }
        #endregion

        #region LftRgt
        if (Input.GetKeyDown(KeyCode.A))
        {
            Lft_FLAG = true;
            Rgt_FLAG = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            Lft_FLAG = false;
            if (Input.GetKey(KeyCode.D))
                Rgt_FLAG = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Lft_FLAG = false;
            Rgt_FLAG = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            Rgt_FLAG = false;
            if (Input.GetKey(KeyCode.A))
                Lft_FLAG = true;
        }
        #endregion

        #region HorizontalAxis_FUNC
        if (Fwd_FLAG)
        {
            emulator.StartMotor(BotEmulator.Polarity.Positive);
            await brick.DirectCommand.SetMotorPolarity(OutputPort.All, Polarity.Forward);
            await brick.DirectCommand.TurnMotorAtSpeedAsync(OutputPort.All, 40);
            SetButtonColor(keyButtons[(int)Buttons.W], new UnityEngine.Color(0, 0, 0, 0.3921569f));
        }
        if (!Fwd_FLAG)
        {
            SetButtonColor(keyButtons[(int)Buttons.W], new UnityEngine.Color(0, 0, 0, 1));
        }

        if (Bwd_FLAG)
        {
            emulator.StartMotor(BotEmulator.Polarity.Negative);
            await brick.DirectCommand.SetMotorPolarity(OutputPort.All, Polarity.Backward);
            await brick.DirectCommand.TurnMotorAtSpeedAsync(OutputPort.All, 40);
            SetButtonColor(keyButtons[(int)Buttons.S], new UnityEngine.Color(0, 0, 0, 0.3921569f));
        }
        if (!Bwd_FLAG)
        {
            SetButtonColor(keyButtons[(int)Buttons.S], new UnityEngine.Color(0, 0, 0, 1));
        }

        if (!Fwd_FLAG && !Bwd_FLAG && !Input.GetKey(KeyCode.Space))
            emulator.StopMotor(false);
        #endregion

        #region VerticalAxis_FUNC
        if (Lft_FLAG)
        {
            emulator.TurnLeft_UNDEF();
            SetButtonColor(keyButtons[(int)Buttons.A], new UnityEngine.Color(0, 0, 0, 0.3921569f));
        }
        if (!Lft_FLAG)
        {
            SetButtonColor(keyButtons[(int)Buttons.A], new UnityEngine.Color(0, 0, 0, 1));
        }

        if (Rgt_FLAG)
        {
            emulator.TurnRight_UNDEF();
            SetButtonColor(keyButtons[(int)Buttons.D], new UnityEngine.Color(0, 0, 0, 0.3921569f));
        }
        if (!Rgt_FLAG)
        {
            SetButtonColor(keyButtons[(int)Buttons.D], new UnityEngine.Color(0, 0, 0, 1));
        }

        if (!Lft_FLAG && !Rgt_FLAG)
            emulator.ResetTurn_UNDEF();
        #endregion

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ColorBlock block = keyButtons[(int)Buttons.W].colors;
            block.normalColor = new UnityEngine.Color(0, 0, 0, 0.3921569f);
            keyButtons[(int)Buttons.Space].colors = block;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ColorBlock block = keyButtons[(int)Buttons.W].colors;
            block.normalColor = new UnityEngine.Color(0, 0, 0, 1);
            keyButtons[(int)Buttons.Space].colors = block;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            emulator.StopMotor(true);
            await brick.DirectCommand.SetMotorPolarity(OutputPort.All, Polarity.Forward);
            await brick.DirectCommand.StopMotorAsync(OutputPort.All, true);
        }
    }

    void SetButtonColor(Button button, UnityEngine.Color color)
    {
        ColorBlock block = button.colors;
        block.normalColor = color;
        block.highlightedColor = color;
        block.pressedColor = color;
        block.selectedColor = color;
        button.colors = block;
    }

    void Brick_BrickChanged(object sender, BrickChangedEventArgs eventArgs)
    {
        Debug.Log("your code is doo-doo");
    }

    public async Task ToggleHelpDropdown(bool show)
    {
        if (show)
        {
            keyButtons[(int)Buttons.F1].transform.GetChild(1).GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
            SetButtonColor(keyButtons[(int)Buttons.F1], new UnityEngine.Color(0, 0, 0, 0.3921569f));
            await Task.Delay((int)(keyButtons[(int)Buttons.F1].colors.fadeDuration / 10 * 5 * 1000));
            keyButtons[(int)Buttons.F1].transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            keyButtons[(int)Buttons.F1].transform.GetChild(1).GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 90);
            SetButtonColor(keyButtons[(int)Buttons.F1], new UnityEngine.Color(0, 0, 0, 1));
            await Task.Delay((int)(keyButtons[(int)Buttons.F1].colors.fadeDuration / 10 * 5 * 1000));
            keyButtons[(int)Buttons.F1].transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}