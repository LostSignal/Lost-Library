//-----------------------------------------------------------------------
// <copyright file="VirtualCurrencyHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using PlayFab.ClientModels;
    using UnityEngine;

    public delegate void OnVirtualCurrencyChangedDelegate();

    public class VirtualCurrencyHelper
    {
        public event OnVirtualCurrencyChangedDelegate VirtualCurrencyChanged;

        private Dictionary<string, int> virtualCurrencies = new Dictionary<string, int>();
        private Dictionary<string, int> virtualCurrencyRechargeTimes = new Dictionary<string, int>();

        public VirtualCurrencyHelper()
        {
            PF.PlayfabEvents.OnLoginResultEvent += this.PlayfabEvents_OnLoginResultEvent;
            PF.PlayfabEvents.OnGetUserInventoryResultEvent += this.PlayfabEvents_OnGetUserInventoryResultEvent;
            PF.PlayfabEvents.OnGetPlayerCombinedInfoResultEvent += PlayfabEvents_OnGetPlayerCombinedInfoResultEvent;
        }

        public UnityTask<GetUserInventoryResult> RefreshVirtualCurrency()
        {
            return PF.Do(new GetUserInventoryRequest());
        }

        public int this[string virtualCurrencyId]
        {
            get
            {
                int value;
                if (this.virtualCurrencies.TryGetValue(virtualCurrencyId, out value))
                {
                    return value;
                }

                return -1;
            }
        }

        public int GetSecondsToRecharge(string virtualCurrencyId)
        {
            if (this.virtualCurrencyRechargeTimes == null)
            {
                return 0;
            }

            int rechargeFinishedTime = 0;

            if (this.virtualCurrencyRechargeTimes.TryGetValue(virtualCurrencyId, out rechargeFinishedTime))
            {
                return Math.Max(0, rechargeFinishedTime - (int)Time.realtimeSinceStartup);
            }

            return 0;
        }

        public void InternalAddVirtualCurrencyToInventory(string virtualCurrencyId, int amountToAdd)
        {
            if (virtualCurrencyId == "RM" || virtualCurrencyId == "AD")
            {
                return;
            }

            if (this.virtualCurrencies.ContainsKey(virtualCurrencyId) == false)
            {
                Debug.LogErrorFormat("Tried to add unknown virtual currency to inventory {0}", virtualCurrencyId);
                return;
            }

            this.virtualCurrencies[virtualCurrencyId] += amountToAdd;

            this.VirtualCurrencyChanged?.Invoke();
        }

        public void InternalSetVirtualCurrencyToInventory(string virtualCurrencyId, int neValue)
        {
            if (virtualCurrencyId == "RM" || virtualCurrencyId == "AD")
            {
                return;
            }

            if (this.virtualCurrencies.ContainsKey(virtualCurrencyId) == false)
            {
                Debug.LogErrorFormat("Tried to add unknown virtual currency to inventory {0}", virtualCurrencyId);
                return;
            }

            this.virtualCurrencies[virtualCurrencyId] = neValue;

            this.VirtualCurrencyChanged?.Invoke();
        }

        private void UpdateVirtualCurrencies(Dictionary<string, int> virtualCurrencies, Dictionary<string, VirtualCurrencyRechargeTime> rechargeTimes)
        {
            bool currenciesChanged = false;

            if (virtualCurrencies != null)
            {
                currenciesChanged = true;
                this.virtualCurrencies.Clear();

                foreach (var currencyId in virtualCurrencies.Keys)
                {
                    this.virtualCurrencies.Add(currencyId, virtualCurrencies[currencyId]);
                }
            }

            if (rechargeTimes != null)
            {
                currenciesChanged = true;
                this.virtualCurrencyRechargeTimes.Clear();

                foreach (var vc in rechargeTimes.Keys)
                {
                    if (rechargeTimes.ContainsKey(vc))
                    {
                        this.virtualCurrencyRechargeTimes.Add(vc, (int)(Time.realtimeSinceStartup + rechargeTimes[vc].SecondsToRecharge + 1));
                    }
                }

            }

            if (currenciesChanged)
            {
                this.VirtualCurrencyChanged?.Invoke();
            }
        }

        private void PlayfabEvents_OnLoginResultEvent(LoginResult result)
        {
            var payload = result.InfoResultPayload;

            this.UpdateVirtualCurrencies(payload?.UserVirtualCurrency, payload?.UserVirtualCurrencyRechargeTimes);
        }

        private void PlayfabEvents_OnGetUserInventoryResultEvent(GetUserInventoryResult result)
        {
            this.UpdateVirtualCurrencies(result?.VirtualCurrency, result?.VirtualCurrencyRechargeTimes);
        }

        private void PlayfabEvents_OnGetPlayerCombinedInfoResultEvent(GetPlayerCombinedInfoResult result)
        {
            var payload = result.InfoResultPayload;

            this.UpdateVirtualCurrencies(payload?.UserVirtualCurrency, payload?.UserVirtualCurrencyRechargeTimes);
        }
    }
}

#endif
