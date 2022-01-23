using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryLine : MonoBehaviour
{
    [Header("References")]
    public List<RectTransform> stepTransforms;
    public GameObject hero;

    public void MovePlayer(EventQueue queue, int index)
    {
        queue.events.Add(MovePlayerRoutine(queue, index));
    }
    IEnumerator MovePlayerRoutine(EventQueue queue, int index)
    {
        yield return null;
        index++;
        LeanTween.moveLocalX(hero, stepTransforms[index].localPosition.x, 0.8f).setEaseInOutQuint().setOnComplete(value => { queue.UpdateQueue(); } );
    }

    public void ResetPlayerPosition()
    {
        hero.transform.localPosition = stepTransforms[0].localPosition;
    }

    public void HeroContainerAttackFeedBack(EventQueue queue = null)
    {
        float originY = hero.transform.localPosition.y;
        LeanTween.moveLocalY(hero, originY - 2f, 0.5f).setEaseOutQuint().setOnComplete(() => 
        {
            LeanTween.moveLocalY(hero, originY - 50, 0.2f).setEaseInQuint().setOnComplete(() => 
            {
                if (GameManager.Instance.currentHero.audioSource == null) GameManager.Instance.currentHero.audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
                Sound intervert = new Sound(GameManager.Instance.currentHero.audioSource, "SFX_ATKFROMHERO", SoundType.SFX, false, false);
                intervert.Play();

                LeanTween.moveLocalY(hero, originY, 1).setEaseOutQuint().setOnComplete(()=> 
               { 
                   if (queue != null) queue.resolved = true; 
               
               });
            
            });
        
        });
    }

    public void HeroContainerDamageFeedback(EventQueue queue)
    {
        LeanTween.rotateZ(hero, -3, 0.025f).setEaseOutCubic().setOnComplete(
            value => LeanTween.rotateZ(hero, 3, 0.05f).setEaseInOutCubic().setLoopPingPong(2).setOnComplete(
                val => LeanTween.rotateZ(hero, 0, 0.025f).setEaseOutCubic().setOnComplete(end => queue.resolved = true)));
    }
}
