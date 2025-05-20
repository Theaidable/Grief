using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Grief.Classes.DesignPatterns.Composite.Components;

namespace Grief.Classes.DesignPatterns.Composite
{
    public class GameObject
    {
        //Liste af components
        private List<Component> components = new List<Component>();

        //Properties
        public Transform Transform { get; private set; }
        public string Tag { get; set; }

        /// <summary>
        /// Constructor af et gameobject, hvor vi altid tilføjer en transformer
        /// </summary>
        public GameObject()
        {
            Transform = AddComponent<Transform>();
        }

        /// <summary>
        /// Metode til at awake alle components
        /// </summary>
        public void Awake()
        {
            foreach (var component in components)
            {
                component.Awake();
            }
        }

        /// <summary>
        /// Metode til at kalde alle compenents ved start
        /// </summary>
        public void Start()
        {
            foreach (var component in components)
            {
                component.Start();
            }
        }

        /// <summary>
        /// Metoden opdaterer alle components
        /// </summary>
        public void Update()
        {
            foreach (var component in components)
            {
                component.Update();
            }
        }

        /// <summary>
        /// Metode som tegener gameobjectet ved at tegne dens components
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var component in components)
            {
                component.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Metode til at tilføje et component til gameobjectet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="additionalParameters"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T AddComponent<T>(params object[] additionalParameters) where T : Component
        {
            Type componentType = typeof(T);
            try
            {
                // Opret en instans ved hjælp af den fundne konstruktør og leverede parametre
                object[] allParameters = new object[1 + additionalParameters.Length];
                allParameters[0] = this;
                Array.Copy(additionalParameters, 0, allParameters, 1, additionalParameters.Length);

                T component = (T)Activator.CreateInstance(componentType, allParameters);
                components.Add(component);
                return component;
            }

            //Catch hvis der sker en fejl
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fejl ved oprettelse af komponenten {componentType.Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Metode til at tilgå components
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Component
        {
            return components.OfType<T>().FirstOrDefault();
        }
    }
}
