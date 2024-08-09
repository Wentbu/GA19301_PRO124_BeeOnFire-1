using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NPC : MonoBehaviour
{
    public GameObject DialoguePanel;  // Bảng đối thoại
    public Text DialogueText;  // Văn bản hiển thị đối thoại
    public Text examQ;  // Văn bản câu hỏi hiển thị đối thoại
    [SerializeField] Animator animator;
    public string[] dialogue;  // Mảng chứa các dòng đối thoại
    private string[] exam; // Mảng chứa câu hỏi
    private string[][] answers; // Mảng chứa các câu trả lời
    private int index;  // Chỉ số của dòng đối thoại hiện tại

    [SerializeField] PlayerHealth Heal;
    [SerializeField] private GameObject QuestionMask;
    public GameObject continueButton;  // Nút tiếp tục
    public float wordSpeed;  // Tốc độ hiển thị từng chữ
    public bool playerIsClose;  // Kiểm tra xem người chơi có đang ở gần hay không
    private bool isTyping;  // Kiểm tra xem văn bản có đang được gõ hay không

    public GameObject[] answerButtons;  // Các nút đáp án
    public Image[] answerBorders;  // Khung viền đáp án
    private int[] correctAnswers;  // Các chỉ số đáp án đúng

    public Sprite correctAnswerSprite;  // Hình dấu tích
    public Sprite incorrectAnswerSprite;  // Hình dấu X

    private Coroutine hideDialogueCoroutine; // Coroutine để tự động ẩn bảng đối thoại
    private int randomQuestionIndex;  // Chỉ số của câu hỏi ngẫu nhiên

    private Sprite[] answerButtonSprites;  // Mảng lưu trữ hình ảnh của các button đáp án

    private void Awake()
    {
        Heal = GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        DialoguePanel.SetActive(false);  // Ẩn bảng đối thoại
        continueButton.SetActive(false);  // Ẩn nút tiếp tục
        isTyping = false;  // Đặt cờ isTyping thành false

        // Initialize dialogue
        dialogue = new string[] {
            "BEE ON FIRE!"
        };

        // Initialize questions and answers
        exam = new string[] {
            "Lập trình Game sử dụng ngôn ngữ gì?",
            "Unity là gì?",
            "C# là ngôn ngữ của?",
            "Unreal Engine là công cụ gì?",
            "FPT Polytechnic có ngành Lập trình Game?",
            "Game Engine phổ biến là?",
            "Chuyên ngành Game kéo dài?",
            "Lập trình Game cần kỹ năng?",
            "Lập trình viên Game cần biết?",
            "Game 3D yêu cầu gì?",
            "FPT Polytechnic đào tạo Game?",
            "Thiết kế nhân vật dùng phần mềm?",
            "Game 2D dùng công cụ?",
            "Lập trình Game học môn gì?",
            "Thực tập ngành Game ở?",
            "Học Game có khó không?",
            "Ngôn ngữ lập trình phổ biến?",
            "Game Engine nào mạnh?",
            "FPT Polytechnic có dạy Unity?",
            "Lập trình Game có cần toán?"
        };

        answers = new string[][] {
            new string[] { "C#", "Java", "Python" },
            new string[] { "Game Engine", "Phần mềm", "Công cụ" },
            new string[] { "Microsoft", "Google", "Apple" },
            new string[] { "Game Engine", "IDE", "Framework" },
            new string[] { "Có", "Không", "Chưa rõ" },
            new string[] { "Unity", "Unreal", "Cả hai" },
            new string[] { "2 năm", "2.5 năm", "3 năm" },
            new string[] { "Logic", "Sáng tạo", "Cả hai" },
            new string[] { "C#", "HTML", "CSS" },
            new string[] { "Đồ họa", "Lập trình", "Cả hai" },
            new string[] { "Có", "Không", "Chưa rõ" },
            new string[] { "Photoshop", "Blender", "Illustrator" },
            new string[] { "Unity", "GameMaker", "Photoshop" },
            new string[] { "CSDL", "Giải thuật", "Cả hai" },
            new string[] { "Công ty Game", "Freelancer", "Cả hai" },
            new string[] { "Có", "Không", "Khó vừa" },
            new string[] { "C#", "Java", "Python" },
            new string[] { "Unity", "Unreal", "Godot" },
            new string[] { "Có", "Không", "Chưa rõ" },
            new string[] { "Có", "Không", "Chưa rõ" }
        };

        correctAnswers = new int[] {
            0,  // C#
            0,  // Game Engine
            0,  // Microsoft
            0,  // Game Engine
            0,  // Có
            2,  // Cả hai
            1,  // 2.5 năm
            2,  // Cả hai
            0,  // C#
            2,  // Cả hai
            0,  // Có
            1,  // Blender
            0,  // Unity
            2,  // Cả hai
            2,  // Cả hai
            0,  // Có
            0,  // C#
            1,  // Unreal
            0,  // Có
            0   // Có
        };

        // Khởi tạo mảng lưu trữ hình ảnh của các button đáp án
        answerButtonSprites = new Sprite[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Image buttonImage = answerBorders[i];
            answerButtonSprites[i] = buttonImage.sprite;  // Lưu hình ảnh hiện tại của button
        }
    }

    void Update()
    {
        if (!isTyping && DialogueText.text == dialogue[index])
        {
            if (index < dialogue.Length - 1)
            {
                SetAnswerButtonsActive(true);  // Hiển thị các nút đáp án
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && playerIsClose)
        {
            if (DialoguePanel.activeInHierarchy)
            {
                StopHideDialogueCoroutine(); // Dừng coroutine ẩn bảng đối thoại nếu đang chạy
                zeroText();  // Đặt lại văn bản nếu bảng đối thoại đang hiển thị
            }
            else
            {
                randomQuestionIndex = Random.Range(0, exam.Length);  // Lấy ngẫu nhiên một câu hỏi
                DialoguePanel.SetActive(true);  // Hiển thị bảng đối thoại
                StartCoroutine(Typing());  // Bắt đầu gõ văn bản
                hideDialogueCoroutine = StartCoroutine(HideDialogueAfterTime(50f));  // Bắt đầu coroutine tự động ẩn bảng đối thoại sau 50 giây
            }
        }
    }

    // Coroutine để ẩn bảng đối thoại sau một khoảng thời gian
    private IEnumerator HideDialogueAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        zeroText();
    }

    private void StopHideDialogueCoroutine()
    {
        if (hideDialogueCoroutine != null)
        {
            StopCoroutine(hideDialogueCoroutine);
            hideDialogueCoroutine = null;
        }
    }

    public void zeroText()
    {
        StopAllCoroutines();  // Dừng tất cả coroutine
        DialogueText.text = "";
        examQ.text = "";
        index = 0;
        DialoguePanel.SetActive(false);
        continueButton.SetActive(false);
        isTyping = false;
    }

    IEnumerator Typing()
    {
        isTyping = true;
        DialogueText.text = "";
        examQ.text = "";
        foreach (char letter in dialogue[index].ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        yield return StartCoroutine(QuestionEffect());

        foreach (char ques in exam[randomQuestionIndex].ToCharArray())
        {
            examQ.text += ques;
            yield return new WaitForSeconds(wordSpeed);
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Text answerText = answerButtons[i].GetComponentInChildren<Text>();
            answerText.text = answers[randomQuestionIndex][i];
        }

        isTyping = false;
    }

    // Coroutine để tạo hiệu ứng vòng quay cho câu hỏi và các nút đáp án
    IEnumerator QuestionEffect()
    {
        float duration = 1.5f;  // Thời gian hiệu ứng
        float elapsed = 0f;

        while (elapsed < duration)
        {
            int tempIndex = Random.Range(0, exam.Length);
            examQ.text = exam[tempIndex];
            for (int i = 0; i < answerButtons.Length; i++)
            {
                Text answerText = answerButtons[i].GetComponentInChildren<Text>();
                answerText.text = ((char)('A' + i)).ToString();  // Đặt nội dung là A, B, C
            }
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        examQ.text = ""; // Xóa văn bản câu hỏi sau khi vòng quay kết thúc
    }

    public void NextLine()
    {
        continueButton.SetActive(false);
        if (index < dialogue.Length - 1)
        {
            index++;
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();  // Đặt lại văn bản khi hết dòng đối thoại
            this.enabled = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();  // Đặt lại văn bản khi người chơi rời vùng kích hoạt
        }
    }

    private void SetAnswerButtonsActive(bool isActive)
    {
        foreach (GameObject button in answerButtons)
        {
            button.SetActive(isActive);
        }
    }

    public void SelectAnswer(int answerIndex)
    {
        // Debug để kiểm tra giá trị answerIndex
        Debug.Log("Selected answer index: " + answerIndex);

        // Kiểm tra độ dài của mảng correctAnswers
        if (answerIndex >= 0 && answerIndex < correctAnswers.Length)
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                Image buttonImage = answerBorders[i];  // Lấy hình ảnh của khung viền đáp án

                if (i == answerIndex)
                {
                    if (i == correctAnswers[randomQuestionIndex])
                    {
                        buttonImage.sprite = correctAnswerSprite;  // Đặt hình ảnh là dấu tích
                    }
                    else
                    {
                        buttonImage.sprite = incorrectAnswerSprite;  // Đặt hình ảnh là dấu X
                    }
                }
                else
                {
                    buttonImage.sprite = null;  // Loại bỏ hình ảnh cho các nút khác
                }
            }

            StartCoroutine(ShowCorrectAnswerFeedback(answerIndex));
        }
        else
        {
            Debug.LogError("Invalid answer index: " + answerIndex);
        }
    }

    private IEnumerator ShowCorrectAnswerFeedback(int answerIndex)
    {
        yield return new WaitForSeconds(0.5f);  // Chờ một khoảng thời gian ngắn để hiển thị hình ảnh dấu tích

        // Kiểm tra nếu người dùng đã chọn đúng đáp án
        if (answerIndex == correctAnswers[randomQuestionIndex])
        {
            SetAnswerButtonsActive(false);  // Tắt các nút đáp án
            continueButton.SetActive(true);  // Hiển thị nút tiếp tục

            // Thay đổi nội dung của examQ thành thông báo chúc mừng
            examQ.text = "Chúc mừng, bạn đã trả lời đúng! Hãy tiếp tục.";
        }
        else
        {
            StartCoroutine(HideIncorrectAnswer());  // Nếu trả lời sai, ẩn câu hỏi và hiển thị lại
        }
    }


    private IEnumerator HideIncorrectAnswer()
    {
        yield return new WaitForSeconds(1f);  // Thời gian chờ để hiển thị dấu X

        // Loại bỏ hình ảnh dấu X
        foreach (Image border in answerBorders)
        {
            border.sprite = null;
        }

        SetAnswerButtonsActive(true); // Hiển thị lại các nút đáp án

        // Khôi phục hình ảnh cho các button đáp án từ mảng answerButtonSprites
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Image buttonImage = answerBorders[i];
            buttonImage.sprite = answerButtonSprites[i];
        }

        zeroText();  // Đặt lại văn bản khi chọn sai đáp án
        DialoguePanel.SetActive(true);  // Hiển thị lại bảng đối thoại
        randomQuestionIndex = Random.Range(0, exam.Length);  // Lấy ngẫu nhiên một câu hỏi mới
        StartCoroutine(Typing());
    }
}
