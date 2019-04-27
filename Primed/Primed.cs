using System;
using System.Collections.Generic;
using InfinityScript;

public class Primed : BaseScript
{
    List<Entity> IntroCompleters = new List<Entity>();
    List<Entity> Primers = new List<Entity>();
    List<Entity> SpawnedPlayers = new List<Entity>();
    public Primed()
    {
        GSCFunctions.PreCacheItem("uav_strike_missile_mp");
        PlayerConnected += Prime_PlayerConnected;

        OnInterval(100, () =>
        {
            if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1)
            {
                startHUD();
                return false;
            }
            return true;
        });
    }

    void Prime_PlayerConnected(Entity player)
    {
        player.SetClientDvar("cg_objectiveText", "Don't die. Primed players explode upon death, so watch out!");
        player.OnNotify("joined_team", entity =>
        {
            player.CloseInGameMenu();
            player.Notify("menuresponse", "changeclass", "axis_recipe1");
        });
        player.SpawnedPlayer += () => OnPlayerSpawned(player);
        //obj.SetField("Primed", false);
        //obj.SetField("IntroDone", 0);
    }
    public void OnPlayerSpawned(Entity player)
    {
        player.SetClientDvar("cg_objectiveText", "Don't die. Primed players explode upon death, so watch out!");
        if (!SpawnedPlayers.Contains(player))
            SpawnedPlayers.Add(player);

        player.AllowSprint(false);
        determinePrime(player);
        introSequence(player);
        AfterDelay(30000, () =>
            {
                OnInterval(1000, () =>
                    {
                        if (Primers.Count == 0)
                        {
                            player.IPrintLnBold("^2No Primed players left!  ^1FREE-FOR-ALL!");
                            player.AllowSprint(true);
                            return false;
                        }
                        return true;
                    });
            });
    }
    public void determinePrime(Entity player)
    {
        int? prime = new Random().Next(3);
        switch (prime)
        {
            case 1:
                //player.SetField("Primed", true);
                Primers.Add(player);
                break;
        }
    }
    public override void OnPlayerDisconnect(Entity player)
    {
        SpawnedPlayers.Remove(player);
        if (Primers.Contains(player))
        {
            Primers.Remove(player);
        }
        if (IntroCompleters.Contains(player))
        {
            IntroCompleters.Remove(player);
        }
    }
    public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
    {
        if (Primers.Contains(player))
        {
            GSCFunctions.MagicBullet("uav_strike_missile_mp", player.Origin, player.Origin - new Vector3(0, 0, 4), player);
            Primers.Remove(player);
        }
    }
    public void introSequence(Entity player)
    {
        player.FreezeControls(true);
        HudElem wait = HudElem.CreateFontString(player, HudElem.Fonts.HudBig, 1.5f);
        wait.Alpha = 0;
        wait.SetPoint("CENTER", "CENTER", 0, 160);
        wait.HideWhenInMenu = false;
        wait.GlowAlpha = 1;
        wait.GlowColor = new Vector3(1, 0, 0);
        OnInterval(1800, () =>
        {
            if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1) return false;

            if (Players.Count < 2)
            {
                wait.SetText("Waiting for other players");
                AfterDelay(300, () =>
                     wait.SetText("Waiting for other players."));
                AfterDelay(600, () =>
                    wait.SetText("Waiting for other players.."));
                AfterDelay(900, () =>
                    wait.SetText("Waiting for other players..."));
                AfterDelay(1200, () =>
                    wait.SetText("Waiting for other players.."));
                AfterDelay(1500, () =>
                    wait.SetText("Waiting for other players."));
                AfterDelay(1800, () =>
                    wait.SetText("Waiting for other players"));
                return true;
            }
            else
            {
                wait.SetText("Waiting for other players to be ready");
                AfterDelay(300, () =>
                     wait.SetText("Waiting for other players to be ready."));
                AfterDelay(600, () =>
                    wait.SetText("Waiting for other players to be ready.."));
                AfterDelay(900, () =>
                    wait.SetText("Waiting for other players to be ready..."));
                AfterDelay(1200, () =>
                    wait.SetText("Waiting for other players to be ready.."));
                AfterDelay(1500, () =>
                    wait.SetText("Waiting for other players to be ready."));
                AfterDelay(1800, () =>
                    wait.SetText("Waiting for other players to be ready"));
                return true;
            }
        });
        OnInterval(100, () =>
        {
            if (IntroCompleters.Count != SpawnedPlayers.Count)
            {
                player.VisionSetNakedForPlayer("black_bw", 1);
                player.FreezeControls(true);
                return true;
            }
            else if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1)
            {
                player.VisionSetNakedForPlayer("cobra_sunset3", 1);
                player.FreezeControls(false);
                wait.FadeOverTime(1);
                wait.Alpha = 0;
                return false;
            }
            return true;
        });
        HudElem title = HudElem.CreateFontString(player, HudElem.Fonts.HudSmall, 1.5f);
        title.Alpha = 0;
        title.SetPoint("CENTER", "CENTER", 0, -150);
        title.HideWhenInMenu = false;
        title.SetText("^2Primed...");
        title.GlowAlpha = 1;
        title.GlowColor = new Vector3(1, 0, 0);
        title.FadeOverTime(1);
        title.Alpha = 1;
        OnInterval(100, () =>
        {
            if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1)
            {
                title.FadeOverTime(1);
                title.Alpha = 0;
                return false;
            }
            return true;
        });

        AfterDelay(1000, () =>
            {
                HudElem text1 = HudElem.CreateFontString(player, HudElem.Fonts.HudBig, 1.2f);
                text1.Alpha = 0;
                text1.SetPoint("CENTER", "CENTER", 0, -20);
                text1.HideWhenInMenu = false;
                OnInterval(500, () =>
                        {
                            text1.SetText(string.Format("^1There are {0} players in this game.", SpawnedPlayers.Count));
                            if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1) return false;
                            return true;
                        });
                text1.GlowAlpha = 1;
                text1.GlowColor = new Vector3(1, 0, 0);
                text1.FadeOverTime(1);
                text1.Alpha = 1;
                OnInterval(100, () =>
                    {
                        if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1)
                        {
                            text1.FadeOverTime(1);
                            text1.Alpha = 0;
                            return false;
                        }
                        return true;
                    });
            });

        AfterDelay(3000, () =>
            {
                HudElem text2 = HudElem.CreateFontString(player, HudElem.Fonts.HudBig, 1.2f);
                text2.Alpha = 0;
                text2.SetPoint("CENTER", "CENTER", 0, 10);
                text2.HideWhenInMenu = false;
                OnInterval(500, () =>
                    {
                        text2.SetText(string.Format("^1{0} of them are Primed.", Primers.Count));
                        if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1) return false;
                        return true;
                    });
                text2.GlowAlpha = 1;
                text2.GlowColor = new Vector3(1, 0, 0);
                text2.FadeOverTime(1);
                text2.Alpha = 1;
                OnInterval(100, () =>
                {
                    if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1)
                    {
                        text2.FadeOverTime(1);
                        text2.Alpha = 0;
                        return false;
                    }
                    return true;
                });
            });

        AfterDelay(5000, () =>
            {
                HudElem warn = HudElem.CreateFontString(player, HudElem.Fonts.HudBig, 1.6f);
                warn.Alpha = 0;
                warn.SetPoint("CENTER", "CENTER", 0, 50);
                warn.HideWhenInMenu = false;
                warn.SetText("^1Watch Out!");
                warn.GlowAlpha = 1;
                warn.GlowColor = new Vector3(1, 0, 0);
                warn.FadeOverTime(1);
                warn.Alpha = 1;
                OnInterval(100, () =>
                {
                    if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1)
                    {
                        warn.FadeOverTime(1);
                        warn.Alpha = 0;
                        return false;
                    }
                    return true;
                });
            });

        AfterDelay(7000, () =>
            {
                IntroCompleters.Add(player);
                wait.FadeOverTime(1);
                wait.Alpha = 1;
                OnInterval(100, () =>
                {
                    if (IntroCompleters.Count == SpawnedPlayers.Count && SpawnedPlayers.Count > 1)
                    {
                        wait.FadeOverTime(1);
                        wait.Alpha = 0;
                        return false;
                    }
                    return true;
                });
            });
    }

    public void startHUD()
    {
        HudElem primed = HudElem.CreateServerFontString(HudElem.Fonts.HudBig, 1);
        primed.SetPoint("TOP RIGHT", "TOPRIGHT", -45, 5);
        primed.HideWhenInMenu = true;
        primed.SetText("Primed Players:");
        primed.GlowAlpha = 0.10f;
        primed.GlowColor = new Vector3(0, 0, 0.7f);

        HudElem number = HudElem.CreateServerFontString(HudElem.Fonts.HudBig, 1);
        number.SetPoint("TOP RIGHT", "TOPRIGHT", -5, 5);
        number.HideWhenInMenu = true;
        OnInterval(100, () =>
            {
                number.SetValue(Primers.Count);
                if (Primers.Count == 0) return false;
                return true;
            });
        number.GlowAlpha = 0.10f;
        number.GlowColor = new Vector3(0, 0, 0.7f);
    }
}

