using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Designer.Components
{
    public abstract class PropertyContainer : IDisposable
    {
        private bool mv_fDisposed;
        private readonly Hashtable mv_htblPropertyValues;

        protected PropertyContainer()
        {
            mv_htblPropertyValues = new Hashtable();
        }

        #region Verhalten zum Lesen und Schreiben von Eigenschaften

        /// <summary>
        /// Ermittelt den Wert einer Eigenschaft.
        /// </summary>
        protected object GetValue(string Name)
        {
            if (mv_htblPropertyValues.ContainsKey(Name))
                return mv_htblPropertyValues[Name];

            return null;
        }

        /// <summary>
        /// Ermittelt den Wert einer Eigenschaft und ermöglicht dabei
        /// die Angabe eines Standardwertes, der geliefert wird, wenn 
        /// der Wert zuvor noch nicht gesetzt worden ist.
        /// </summary>
        protected T GetValue<T>(string Name, T DefaultValue = default(T))
        {
            if (mv_htblPropertyValues.ContainsKey(Name))
                return (T)mv_htblPropertyValues[Name];

            return DefaultValue;
        }

        /// <summary>
        /// Überprüft, ob für eine Eigenschaft ein Wert eingetragen ist.
        /// </summary>
        protected bool HasValue(string Name)
        {
            return mv_htblPropertyValues.ContainsKey(Name);
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        protected void SetValue(string Name, object Value, bool silent = false)
        {
            bool fRaiseChanged = false;

            lock (mv_htblPropertyValues.SyncRoot)
            {
                if (mv_htblPropertyValues.ContainsKey(Name))
                {
                    if (Value == null)
                    {
                        fRaiseChanged = true;
                        mv_htblPropertyValues.Remove(Name);
                    }
                    else
                    {
                        object objPreviousValue = mv_htblPropertyValues[Name];

                        if (objPreviousValue is IComparable)
                        {
                            if (((IComparable)objPreviousValue).CompareTo(Value) != 0)
                                fRaiseChanged = true;
                        }
                        else if (objPreviousValue != Value)
                            fRaiseChanged = true;

                        //Wenn sich etwas verändert hat, wird der neue Wert festgehalten
                        if (fRaiseChanged)
                            mv_htblPropertyValues[Name] = Value;
                    }
                }
                else if (Value != null)
                {
                    mv_htblPropertyValues.Add(Name, Value);
                    fRaiseChanged = true;
                }
            }

            // Wenn der Wert sich verändert hat, wird ein Ereignis ausgelöst
            if (fRaiseChanged && !silent)
                RaisePropertyChanged(Name);
        }

        /// <summary>
        /// Löst das Ereignis zur Anzeige der Änderung eines Eigenschaftswertes aus
        /// </summary>
        protected abstract void RaisePropertyChanged(string PropertyName);

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool Disposing)
        {
            if (!mv_fDisposed)
            {
                this.OnDisposing();
                mv_fDisposed = true;
            }
        }

        protected virtual void OnDisposing()
        {

        }
    }
}
