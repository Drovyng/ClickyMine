using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ParticlesGenerator : MonoBehaviour
{
    private const int MaxParticles = 150;
    private static List<Particle> Buffer = new(MaxParticles);
    private static List<Particle> OnScreen = new(MaxParticles);

    [SerializeField] private GameObject _prefab;

    public static void Generate(int count, Vector2 position, float posRNG, float force, float forceRNG, float timer, float timerRNG, string sprite)
    {
        while (count > 0 && Buffer.Count > 0)
        {
            var part = Buffer[0];
            Buffer.Remove(part);
            OnScreen.Add(part);
            part.Launch(
                AssetLoader.Images[sprite], 
                position + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * posRNG, 
                force + Random.Range(-1f, 1f) * forceRNG, 
                timer + Random.Range(-1f, 1f) * timerRNG
            );
            count--;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BlockTouch(string sprite)
    {
        Generate(4, Vector2.zero, 200, 800, 100, 0.3f, 0.075f, "Blocks/p_"+sprite);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BlockBreak(string sprite)
    {
        Generate(25, Vector2.zero, 250, 1500, 200, 1.1f, 0.2f, "Blocks/p_" + sprite);
    }
    private void Awake()
    {
        for (int i = 0; i < MaxParticles; i++)
        {
            Buffer.Add(Instantiate(_prefab, transform).GetComponent<Particle>());
        }
    }
    private void Update()
    {
        for (int i = 0; i < OnScreen.Count; i++)
        {
            var item = OnScreen[i];
            if (item.UpdateMe(Time.deltaTime))
            {
                OnScreen.Remove(item);
                Buffer.Add(item);
                i--;
            }
        }
    }
}