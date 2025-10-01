using UnityEngine;

public class CardEffect
{
    public EffectEnum currentEffect;
    public CardEffect(EffectEnum effectToCreate)
    {
        currentEffect = effectToCreate;
    }
    public virtual void Effect()
    {

    }
}
