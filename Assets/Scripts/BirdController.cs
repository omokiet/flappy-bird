using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class BirdController : MonoBehaviour
{
    public float flapSpeed = 4f;
    public GameObject readyPanel;
    public GameObject gameOverPanel;
    public PipeSpawner pipeSpawner; 
    public Image[] digitImages = new Image[3];
    public Sprite[] numberSprites = new Sprite[10];

[Header("Bird Tilt")]
    public float tiltUpAngle = 30f;
    public float tiltDownAngle = -30f;
    public float tiltUpDuration = 0.1f;
    public float tiltDownDuration = 0.25f;

[Header("Sound Effects")]
    public AudioClip wingSound;
    public AudioClip pointSound;
    public AudioClip hitSound;
    public AudioClip dieSound;

    private AudioSource audioSource;
    private float tiltTimer;
    private float tiltStartAngle;
    

    private Rigidbody2D rb;
    private int score = 0;
    private bool gameStarted = false;
    private bool isDead = false;

    private void Awake()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        rb.rotation = 0f;
        rb.simulated = false;
        score = 0;
        UpdateScoreDisplay();

        readyPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    private void Update()
{
    if (isDead)
    {
        return;
    }

    bool flapPressed =
        (Keyboard.current != null &&
         Keyboard.current.spaceKey.wasPressedThisFrame) ||
        (Mouse.current != null &&
         Mouse.current.leftButton.wasPressedThisFrame) ||
        (Touchscreen.current != null &&
         Touchscreen.current.primaryTouch.press.wasPressedThisFrame);

    if (flapPressed)
    {
        if (!gameStarted)
        {
            gameStarted = true;
            readyPanel.SetActive(false);
            rb.simulated = true;
            pipeSpawner.StartSpawning();
        }

        rb.linearVelocity = Vector2.up * flapSpeed;

        tiltStartAngle = rb.rotation;
        tiltTimer = 0f;

        PlaySound(wingSound);
    }

    if (gameStarted)
    {
        UpdateTilt();
    }
}

    public void AddScore()
{
    if (!gameStarted || isDead)
    {
        return;
    }

    score++;
    UpdateScoreDisplay();
    PlaySound(pointSound);
}

    private void UpdateTilt()
{
    float upDuration = Mathf.Max(0.01f, tiltUpDuration);
    float downDuration = Mathf.Max(0.01f, tiltDownDuration);

    tiltTimer = Mathf.Min(
        tiltTimer + Time.deltaTime,
        upDuration + downDuration
    );

    float angle;

    if (tiltTimer <= upDuration)
    {
        float progress = tiltTimer / upDuration;
        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

        angle = Mathf.LerpAngle(
            tiltStartAngle,
            tiltUpAngle,
            smoothProgress
        );
    }
    else
    {
        float progress = (tiltTimer - upDuration) / downDuration;
        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

        angle = Mathf.LerpAngle(
            tiltUpAngle,
            tiltDownAngle,
            smoothProgress
        );
    }

    rb.rotation = angle;
}

private void PlaySound(AudioClip clip)
{
    if (audioSource != null && clip != null)
    {
        audioSource.PlayOneShot(clip);
    }
}

private IEnumerator PlayDeathSound()
{
    yield return new WaitForSecondsRealtime(0.12f);
    PlaySound(dieSound);
}

    private void UpdateScoreDisplay()
    {
        if (digitImages == null || numberSprites == null || numberSprites.Length < 10)
        {
            return;
        }

        string scoreText = score.ToString();
        int visibleDigits = Mathf.Min(scoreText.Length, digitImages.Length);

        for (int i = 0; i < digitImages.Length; i++)
        {
            if (digitImages[i] == null)
            {
                continue;
            }

            int scoreIndex = scoreText.Length - digitImages.Length + i;

            if (scoreIndex < 0)
            {
                digitImages[i].enabled = false;
                continue;
            }

            digitImages[i].enabled = true;
            int digit = scoreText[scoreIndex] - '0';
            digitImages[i].sprite = numberSprites[digit];

            RectTransform rect = digitImages[i].rectTransform;
            int visibleIndex = i - (digitImages.Length - visibleDigits);
            Vector2 position = rect.anchoredPosition;
            position.x = (visibleIndex - (visibleDigits - 1) * 0.5f) * (rect.rect.width + 10f);
            rect.anchoredPosition = position;
        }
    }

   private void OnCollisionEnter2D(Collision2D collision)
{
    if (!gameStarted || isDead)
    {
        return;
    }

    isDead = true;

    PlaySound(hitSound);
    StartCoroutine(PlayDeathSound());

    gameOverPanel.SetActive(true);
    Time.timeScale = 0f;
}

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
