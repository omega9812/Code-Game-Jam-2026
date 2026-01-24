using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC;

public class CutsceneManager : MonoBehaviour
{
    [Header("Characters")]
    public Transform bobTransform;
    public Transform clownTransform;
    
    [Header("Effects")]
    public GameObject waterSprayEffect;
    public GameObject dialogueBox;
    
    [Header("Positions")]
    public Transform bobStartPosition;
    public Transform bobEndPosition;
    public Transform clownHidingPosition;
    
    [Header("Dialogue")]
    public string clownLaughText = "Hahaha! Got you!";
    public string clownInviteText = "Hey, come to the fair with me!";
    public UnityEngine.UI.Text dialogueText;
    
    private Animator bobAnimator;
    private Animator clownAnimator;
    private bool cutsceneStarted = false;
    
    void Start()
    {
        // Get references
        if (bobTransform != null && bobTransform.GetComponentInChildren<Animator>() != null)
        {
            bobAnimator = bobTransform.GetComponentInChildren<Animator>();
        }
        
        if (clownTransform != null && clownTransform.GetComponentInChildren<Animator>() != null)
        {
            clownAnimator = clownTransform.GetComponentInChildren<Animator>();
        }
        
        // Set initial positions
        if (bobTransform != null && bobStartPosition != null)
        {
            bobTransform.position = bobStartPosition.position;
        }
        
        if (clownTransform != null && clownHidingPosition != null)
        {
            clownTransform.position = clownHidingPosition.position;
        }
        
        // Hide dialogue box initially
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
        
        // Start the cutscene
        StartCoroutine(PlayCutscene());
    }
    
    IEnumerator PlayCutscene()
    {
        if (cutsceneStarted)
            yield break;
            
        cutsceneStarted = true;
        
        // 1. Bob walks down the street
        if (bobAnimator != null)
        {
            bobAnimator.SetBool("isWalking", true);
        }
        
        float walkDuration = 3.0f;
        float elapsedTime = 0f;
        Vector3 startPos = bobTransform.position;
        Vector3 endPos = bobEndPosition.position;
        
        while (elapsedTime < walkDuration)
        {
            bobTransform.position = Vector3.Lerp(startPos, endPos, elapsedTime / walkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        bobTransform.position = endPos;
        
        // 2. Bob stops walking
        if (bobAnimator != null)
        {
            bobAnimator.SetBool("isWalking", false);
        }
        
        yield return new WaitForSeconds(1.0f);
        
        // 3. Clown sprays water
        if (waterSprayEffect != null)
        {
            waterSprayEffect.SetActive(true);
        }
        
        yield return new WaitForSeconds(1.5f);
        
        // 4. Bob reacts (damaged animation)
        if (bobAnimator != null)
        {
            bobAnimator.SetTrigger("damaged");
        }
        
        if (waterSprayEffect != null)
        {
            waterSprayEffect.SetActive(false);
        }
        
        yield return new WaitForSeconds(1.0f);
        
        // 5. Clown laughs
        if (clownAnimator != null)
        {
            clownAnimator.SetTrigger("laugh");
        }
        
        // 6. Show dialogue
        if (dialogueBox != null && dialogueText != null)
        {
            // First line - Clown laughing
            dialogueBox.SetActive(true);
            dialogueText.text = clownLaughText;
            yield return new WaitForSeconds(2.0f);
            
            // Second line - Clown inviting Bob to the fair
            dialogueText.text = clownInviteText;
            yield return new WaitForSeconds(3.0f);
            
            dialogueBox.SetActive(false);
        }
        
        yield return new WaitForSeconds(1.0f);
        
        Debug.Log("Cutscene completed");
    }
}