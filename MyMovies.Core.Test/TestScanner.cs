using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper;
using NUnit.Framework;
using Monstro.Util;

namespace Test
{
    [TestFixture]
    public class TestScanner
    {
        public class TestResult
        {
            public MovieInfos Movie{ get; set;}
            public bool Ok{ get; set;}
            public TestResult(bool ok, MovieInfos movie)
            {
                Ok = ok;
                Movie = movie;
            }
        }

        public static TestResult[] ScanResult1 = new[]{
            new TestResult(true, new MovieInfos("50 First Dates", null, @"\!A ne pas manquer ;o)\50_First_Dates.FRENCH.DVDRip.XViD-LiQUiDE-teste.DivXovore.com-.avi", false, false)),
            new TestResult(true, new MovieInfos("Crazy Kung-fu", null, @"\!A ne pas manquer ;o)\Crazy.Kung-fu.(Kung.Fu.Hustle).DVDRip.[H264.He-Aac.{Chi-Fr-Eng}.St{Fr-Eng}.Chaps].mkv", false, false)),
            new TestResult(true, new MovieInfos("Eternal Sunshine Of The Spotless Mind", null, @"\!A ne pas manquer ;o)\Eternal.Sunshine.Of.The.Spotless.Mind.FRENCH.DVDRip.XViD-EVASION.shared.by.[TFT].avi", false, false)),
            new TestResult(true, new MovieInfos("festen", null, @"\!A ne pas manquer ;o)\festen(divX fr).avi", false, false)),
            new TestResult(true, new MovieInfos("Fighter In The Wind", null, @"\!A ne pas manquer ;o)\Fighter.In.The.Wind.FRENCH.LiMiTED.DVDRip-XViD-KYoKuSHiN.avi", false, false)),
            new TestResult(true, new MovieInfos("Film Drame - Les Evadés", 1995, @"\!A ne pas manquer ;o)\Film (Stephen King) Drame - Les Evadés (The Shawshank Redemption) Fr Dvd Rip (01 Mars 1995).avi", false, false)),
            new TestResult(true, new MovieInfos("Hotel Rwanda", null, @"\!A ne pas manquer ;o)\Hotel.Rwanda.avi", false, false)),
            new TestResult(true, new MovieInfos("La Cite de dieu", null, @"\!A ne pas manquer ;o)\La.Cite.de.dieu_DVDRip_FR_divx5.0.teste.divxonweb.fr.st.AVI", false, false)),
            new TestResult(true, new MovieInfos("La Guerre Des Mondes", null, @"\!A ne pas manquer ;o)\La.Guerre.Des.Mondes.FRENCH.DVDRIP-XVID-GRRRR.avi", false, false)),
            new TestResult(true, new MovieInfos("Le Pianiste", null, @"\!A ne pas manquer ;o)\Le Pianiste Divx Fr Certifié Par Cyril.avi", false, false)),
            new TestResult(true, new MovieInfos("Le vieux fusil", null, @"\!A ne pas manquer ;o)\Le vieux fusil [rip] [French DVD] [found via www.fileDonkey..avi", false, false)),
            new TestResult(true, new MovieInfos("Les indestructibles", null, @"\!A ne pas manquer ;o)\Les.indestructibles.FRENCH.DVDRip.by.malcom.X.teste.www.divxonweb.fr.st.avi", false, false)),
            new TestResult(true, new MovieInfos("Les Noces Funebres De Tim Burton", null, @"\!A ne pas manquer ;o)\Les.Noces.Funebres.De.Tim.Burton.FRENCH.DVDRiP.XviD.2nD.Share.By.LL75.avi", false, false)),
            new TestResult(true, new MovieInfos("Million Dollar Baby", null, @"\!A ne pas manquer ;o)\Million_Dollar_Baby.FRENCH.FS.DVDRip-XViD-Fuck.les.FS.avi", false, false)),
            new TestResult(true, new MovieInfos("Osmose", null, @"\!A ne pas manquer ;o)\Osmose.FRENCH.DVDRip.XViD-NTK-AvRiL.avi", false, false)),
            new TestResult(true, new MovieInfos("Ratatouille", null, @"\!A ne pas manquer ;o)\Ratatouille.FRENCH.DVDRip.XviD.avi", false, false)),
            new TestResult(true, new MovieInfos("Ray", null, @"\!A ne pas manquer ;o)\Ray.FRENCH.DVDRip.DIVX.REPACK.1CD.1kP-TEAM.avi", false, false)),
            new TestResult(true, new MovieInfos("Requiem For A Dream", null, @"\!A ne pas manquer ;o)\Requiem For A Dream - Fr.avi", false, false)),
            new TestResult(true, new MovieInfos("Star Wars Episode 3 Revenge Of The Sith", null, @"\!A ne pas manquer ;o)\Star.Wars.Episode.3.Revenge.Of.The.Sith.FRENCH.DVDRiP.XViD-GeT-THe-DaRK-SiDe-CD1.avi", false, false)),
            new TestResult(true, new MovieInfos("Vol au-dessus d'un nid de coucou", null, @"\!A ne pas manquer ;o)\Vol.au-dessus.d'un.nid.de.coucou(DivX.fr).avi", false, false)),
            new TestResult(true, new MovieInfos("Willow", null, @"\!A ne pas manquer ;o)\Willow DVDrip Fr.avi", false, false)),
            new TestResult(true, new MovieInfos("30 days of night", null, @"\!HD\30 days of night.mkv", false, false)),
            new TestResult(true, new MovieInfos("300", null, @"\!HD\300.FRENCH.HD-DVDRiP.720p.VF.avi", false, false)),
            new TestResult(true, new MovieInfos("American gangster", null, @"\!HD\American gangster.mkv", false, false)),
            new TestResult(true, new MovieInfos("Braveheart", null, @"\!HD\Braveheart.720p.french.hdtvrip.rmx.pour.Hdfrench.mkv", false, false)),
            new TestResult(true, new MovieInfos("In the Valley of Elah", 2007, @"\!HD\In.the.Valley.of.Elah.2007.720p.BluRay.x264-ESiR.mkv", false, false)),
            new TestResult(true, new MovieInfos("Kingdom Of Heaven", 2005, @"\!HD\Kingdom.Of.Heaven.2005.FRENCH.720p.HD.DVDRip.XviD.AC3-CiNEFOX.avi", false, false)),
            new TestResult(true, new MovieInfos("La Haine", null, @"\!HD\La Haine - FR HD DVD Rip x264 720p DTS 5.1.mkv", false, false)),
            new TestResult(true, new MovieInfos("Les evadés", null, @"\!HD\Les evadés 720p vo+vf.mkv", false, false)),
            new TestResult(true, new MovieInfos("Pixar - One Man Band", null, @"\!HD\Pixar - One Man Band (HD 720p).avi", false, false)),
            new TestResult(true, new MovieInfos("Ratatouille", 2007, @"\!HD\Ratatouille.2007.720p.FRENCH.BRDRiP.XViD.AC3-SEPTiC.iNTERNAL.RE.ENC-RUNTiME.avi", false, false)),
            new TestResult(true, new MovieInfos("Spider-Man 3", null, @"\!HD\Spider-Man.3.720p.VO+VF.mkv", false, false)),
            new TestResult(true, new MovieInfos("Star Wars Episode 1 The Phantom Menace", 1999, @"\!HD\Star wars\Star Wars Episode 1 The Phantom Menace (1999) 720P VO.mkv", false, false)),
            new TestResult(true, new MovieInfos("Star Wars Episode 2 Attack of the Clones", 2002, @"\!HD\Star wars\Star.Wars.Episode.2.Attack.of.the.Clones.2002.720p.HDTV.x264-ESiR.mkv", false, false)),
            new TestResult(true, new MovieInfos("STARSHIP TROOPERS", null, @"\!HD\STARSHIP TROOPERS-FRENCH.720p.HD.DVDrip.AC3-FR.avi", false, false)),
            new TestResult(true, new MovieInfos("The Butterfly Effect", 2004, @"\!HD\The.Butterfly.Effect.2004.720p.AC3.5.1.x264-RMZ.avi", false, false)),
            new TestResult(true, new MovieInfos("X-Men 3 The Last Stand", 2006, @"\!HD\X-Men.3.The.Last.Stand.2006.720p.VO.mkv", false, false)),
            new TestResult(true, new MovieInfos("Pink Floyd The Wall", null, @"\[movie]Pink Floyd The Wall (1982)\Pink Floyd The Wall.mkv", false, false)),
            new TestResult(true, new MovieInfos("Babel", null, @"\Babel\Babel\Babel.FRENCH.DVDRiP.XviD-BABEL.CD1.avi", false, false)),
            new TestResult(true, new MovieInfos("BRAQUAGE A L'ANGLAISE", null, @"\BRAQUAGE A L'ANGLAISE.FRENCH.DVDRiP.avi", false, false)),
            new TestResult(true, new MovieInfos("How to Train Your Dragon", 2010, @"\How to Train Your Dragon 2010 1080p BRRip H264 AAC - IceBane (Kingdom Release)\How to Train Your Dragon.mp4", false, false)),
            new TestResult(true, new MovieInfos("How to Train Your Dragon", null, @"\How to Train Your Dragon 2010 1080p BRRip H264 AAC - IceBane (Kingdom Release)\Xbox 360 2 Ch Audio\How to Train Your Dragon 2 Ch Audio.mp4", false, false)),
            new TestResult(true, new MovieInfos("I comme Icare", 1979, @"\I comme Icare - 1979 (H Verneuil-Y.Montand).avi", false, false)),
            new TestResult(true, new MovieInfos("Inception", null, @"\Inception.1080p.BluRay.x264-REFiNED\refined-inception-1080p.mkv", false, false)),
            new TestResult(true, new MovieInfos("Juno", null, @"\Juno.French.DVDRIP.XviD-Menoetios\Juno.French.DVDRIP.XviD-Menoetios.avi", false, false)),
            new TestResult(true, new MovieInfos("La Chute", null, @"\La Chute (2cd)\La.Chute..avi", false, false)),
            new TestResult(false, new MovieInfos("La ChuteCD2", null, @"\La Chute (2cd)\La.ChuteCD2.avi", false, false)),
            new TestResult(true, new MovieInfos("La Controverse De Valladolid", null, @"\La Controverse De Valladolid.avi", false, false)),
            new TestResult(true, new MovieInfos("Les Liens Du Sang", null, @"\Les.Liens.Du.Sang.FRENCH.DVDRIP.REPACK.1CD\Les.Liens.Du.Sang.FRENCH.DVDRIP.REPACK.1CD.avi", false, false)),
            new TestResult(true, new MovieInfos("Nausicaa de la Vallee du Vent", null, @"\Nausicaa.de.la.Vallee.du.Vent.(Kaze.no.tani.no.Naushika).DVDRip.[x264.HP.He-Aac.{Fr-Jpn}+Subs{French}+Chaps+Cover].mkv", false, false)),
            new TestResult(true, new MovieInfos("Prete Moi Ta Main", null, @"\Prete.Moi.Ta.Main.FRENCH.DVDRip.XviD-MP.avi", false, false)),
            new TestResult(true, new MovieInfos("ROMANZO CRIMINALE", 2006, @"\ROMANZO.CRIMINALE-2006-FRENCH-DVDRIP-XVID-ROMANZO.avi", false, false)),
            new TestResult(true, new MovieInfos("The Sentinel", null, @"\The Sentinel.avi", false, false)),
            new TestResult(true, new MovieInfos("The Simpsons Movie", null, @"\The.Simpsons.Movie.FRENCH.DVDRip.avi", false, false)),
            new TestResult(true, new MovieInfos("TROIS COULEURS - BLANC", null, @"\TROIS COULEURS - BLANC.avi", false, false)),
            new TestResult(true, new MovieInfos("Un Prophete", 2009, @"\Un.Prophete.2009.FRENCH.720p.BluRay.x264-FHD\Un.Prophete.2009.FRENCH.720p.BluRay.x264-FHD.mkv", false, false)),
            new TestResult(true, new MovieInfos("X-Men Origins Wolverine", null, @"\X-Men.Origins.Wolverine.720p.nHD.x264-NhaNc3\X-Men.Origins.Wolverine.720p.nHD.x264-NhaNc3.mp4", false, false)),
            new TestResult(true, new MovieInfos("Babel", null, @"\Babel.FRENCH.DVDRiP.XviD-BABEL.CD2.avi", true, false)),
            new TestResult(true, new MovieInfos("Avatar", null, @"d:\DivX\Séries\Avatar\Livre 3 (vo)\Avatar 308 The Puppetmaster [ThV].avi", false, false)),
            new TestResult(true, new MovieInfos("Manderlay", null, @"d:\DivX\films\[XCT].Manderlay.(Lars.Von.Trier).DVDRip.[x264.HP+He.Aac.v2{Fr-Eng}+Subs{Fr}+Chaps+Cover].mkv", false, false)),                                       
        };

        public static TestResult[] ScanResult2 = new[]{
            new TestResult(true, new MovieInfos("Never Let Me Go", 2010, @"\2010.12.28.Never.Let.Me.Go.2010.BluRay.720p.x264.DTS-MySiLU\Never.Let.Me.Go.2010.BluRay.720p.x264.DTS-MySiLU.mkv", false, false)),
            new TestResult(true, new MovieInfos("99 Francs", 2007, @"\99.Francs.2007.720p.BluRay.DTS.x264-DON\99.Francs.2007.720p.BluRay.DTS.x264-DON.mkv", false, false)),
            new TestResult(true, new MovieInfos("A Guide To Recognizing Your Saints", null, @"\A Guide To Recognizing Your Saints LiMiTED FRENCH DVDRiP XViD-FwD\A Guide To Recognizing Your Saints LiMiTED FRENCH DVDRiP XViD-FwD.avi", false, false)),
            new TestResult(true, new MovieInfos("Animal Kingdom", 2010, @"\Animal.Kingdom.2010.720p.BluRay.x264-aAF\aaf-animal.kingdom.2010.720p.mkv", false, false)),
            new TestResult(true, new MovieInfos("Armadillo", 2010, @"\Armadillo.2010.SWESUB.AC3.DVDRip.XviD-OEric\Armadillo.2010.SWESUB.AC3.DVDRip.XviD-OEric.avi", false, false)),
            new TestResult(true, new MovieInfos("complices", null, @"\aymo-complices.french.avi", false, false)),
            new TestResult(true, new MovieInfos("Baraka", 1992, @"\Baraka 1992 1080p BRRip x264 AAC-BeLLBoY (Kingdom-Release)\Baraka 1992 1080p BRRip x264 AAC-BeLLBoY (Kingdom-Release).mp4", false, false)),
            new TestResult(true, new MovieInfos("Black Swan", 2010, @"\Black.Swan.2010.BluRay.1080p.DTS.x264-CHD\Black.Swan.2010.BluRay.1080p.DTS.x264-CHD.mkv", false, false)),
            new TestResult(true, new MovieInfos("Black Swan", 2010, @"\Black.Swan.2010.BluRay.1080p.DTS.x264-CHD\Black.Swan-SAMPLE.mkv", true, true)),
            new TestResult(true, new MovieInfos("Brothers", 2009, @"\Brothers 2009 720p BluRay x264-Felony\f-brothers.720p.mkv", false, false)),
            new TestResult(true, new MovieInfos("Carancho", 2010, @"\Carancho.2010.DVDRip.XviD.AC3-5.1.HORiZON-ArtSubs\Carancho.2010.DVDRip.XviD.AC3-5.1.HORiZON-ArtSubs.avi", false, false)),
            new TestResult(true, new MovieInfos("Carlos", 2010, @"\Carlos.2010.720p.BluRay.x264-AVCHD\avchd-car.2010.720p.mkv", false, false)),
            new TestResult(true, new MovieInfos("Centurion", 2010, @"\Centurion 2010 BRrip 720 x264 [Torrent-Force]\Centurion 2010 BRrip 720 x264 [Torrent-Force].mkv", false, false)),
            new TestResult(true, new MovieInfos("Cinema, Aspirinas e Urubus", 2005, @"\Cinema, Aspirinas e Urubus.2005.DVDRip.XviD-pedr1nho\Cinema, Aspirinas e Urubus.2005.DVDRip.XviD-pedr1nho.avi", false, false)),
            new TestResult(true, new MovieInfos("City of Life and Death", 2009, @"\City.of.Life.and.Death.2009.BluRay.720p.DTS.x264-CHD\City.of.Life.and.Death.2009.BluRay.720p.DTS.x264-CHD.mkv", false, false)),
            new TestResult(true, new MovieInfos("Code Inconnu", null, @"\Code Unknown - Incomplete Tales of Several Journeys (2000, Michael Haneke)\Code Inconnu.avi", false, false)),
            new TestResult(true, new MovieInfos("Crazy Heart", 2009, @"\Crazy Heart 2009 720p BluRay x264-METiS\metis-cheart-720p.mkv", false, false)),
            new TestResult(true, new MovieInfos("De Helaasheid der Dingen", 2009, @"\De Helaasheid der Dingen (2009) DVDRip XviD DivXNL-Team(dutch spoken NL)\De Helaasheid der Dingen (2009) DVDRip XviD DivXNL-Team.avi", false, false)),
            new TestResult(true, new MovieInfos("Des Hommes Et Des Dieux", 2010, @"\Des.Hommes.Et.Des.Dieux.2010.FRENCH.720p.BluRay.x264-LOST\Des.Hommes.Et.Des.Dieux.2010.FRENCH.720p.BluRay.x264-LOST.mkv", false, false)),
            new TestResult(true, new MovieInfos("Doubt", 2008, @"\Doubt[2008]DvDrip[Eng]-FXG\Doubt[2008]DvDrip[Eng]-FXG.avi", false, false)),
            new TestResult(true, new MovieInfos("Volt Star Malgre Lui", null, @"\final_Volt.Star.Malgre.Lui.FRENCH.DVDRiP.XviD-ULTRASON\volt_xvid.avi", false, false)),
            new TestResult(true, new MovieInfos("Fish Tank", null, @"\Fish Tank LiMiTED DVD www.IWANNADOWNLOAD.com\Fish Tank DVD www.IWANNADOWNLOAD.com.avi", false, false)),
            new TestResult(true, new MovieInfos("Four Lions", 2010, @"\Four Lions 2010 Limited 1080p BRRip H264 AAC - IceBane (Kingdom Release)\Four Lions.mp4", false, false)),
            new TestResult(true, new MovieInfos("Gegen Die Wand", 2004, @"\Gegen die Wand\Gegen.Die.Wand.CD1.2004.DVDRip.XviD.AC3-TURKiSO.avi", false, false)),
            new TestResult(true, new MovieInfos("Gegen Die Wand", 2004, @"\Gegen die Wand\Gegen.Die.Wand.CD2.2004.DVDRip.XviD.AC3-TURKiSO.avi", true, false)),
            new TestResult(true, new MovieInfos("Goal! The Dream Begins", null, @"\Goal!.The.Dream.Begins.DvDrip[Eng]-aXXo.avi", false, false)),
            new TestResult(true, new MovieInfos("Good Morning England", 2009, @"\Good.Morning.England.DVDRIP.FRENCH.2009.[Lucas007dbs].avi", false, false)),
            new TestResult(true, new MovieInfos("Green Zone", null, @"\Green Zone 720p Bluray x264-CBGB\cbgb-greenzone720.mkv", false, false)),
            new TestResult(true, new MovieInfos("inf-tbtr", null, @"\inf-tbtr.mkv", false, false)),
            new TestResult(true, new MovieInfos("Into The Wild", 2007, @"\Into The Wild 2007 720p BRRip H.264 AAC-TheFalcon007(Kingdom-Release)\Into The Wild 2007 720p BRRip H.264 AAC-TheFalcon007(Kingdom-Release).mp4", false, false)),
            new TestResult(true, new MovieInfos("Kerity La Maison Des Contes", null, @"\Kerity.La.Maison.Des.Contes.FRENCH.DVDRip.XViD-DVDFR\Kerity.La.Maison.Des.Contes.FRENCH.DVDRip.XViD-DVDFR.avi", false, false)),
            new TestResult(true, new MovieInfos("L Affaire Farewell", null, @"\L.Affaire.Farewell.FRENCH.BDRip.XviD-Carion\L.Affaire.Farewell.FRENCH.BDRip.XviD-Carion.cd1.avi", false, false)),
            new TestResult(true, new MovieInfos("L Affaire Farewell", null, @"\L.Affaire.Farewell.FRENCH.BDRip.XviD-Carion\L.Affaire.Farewell.FRENCH.BDRip.XviD-Carion.cd2.avi", true, false)),
            new TestResult(true, new MovieInfos("Claude Lelouch La Belle Histoire", null, @"\la belle histoire\Claude Lelouch La Belle Histoire.avi", false, false)),
            new TestResult(true, new MovieInfos("Claude Lelouche La Belle Histoire", null, @"\la belle histoire\Claude Lelouche La Belle Histoire, Cd 2.avi", true, false)),
            new TestResult(true, new MovieInfos("Largo Winch", null, @"\Largo.Winch.FRENCH.SUBFORCED.DVDRiP\Largo.Winch.FRENCH.SUBFORCED.DVDRiP.XviD-PaGlop.avi", false, false)),
            new TestResult(true, new MovieInfos("LE CHATEAU AMBULANT", null, @"\LE CHATEAU AMBULANT avi french dvdrip XviD [ lanesra13 ].avi", false, false)),
            new TestResult(true, new MovieInfos("Le Ruban Blanc", null, @"\Le.Ruban.Blanc FRENCH.DVDRip.XviD-UNSKiLLED\Le.Ruban.Blanc.CD1.FRENCH.DVDRip.XviD-UNSKiLLED.avi", false, false)),
            new TestResult(true, new MovieInfos("Le Ruban Blanc", null, @"\Le.Ruban.Blanc FRENCH.DVDRip.XviD-UNSKiLLED\Le.Ruban.Blanc.CD2.FRENCH.DVDRip.XviD-UNSKiLLED.avi", true, false)),
            new TestResult(true, new MovieInfos("Le Vilain", 2009, @"\Le.Vilain.2009.FRENCH.720p.BluRay.x264-FHD\Le.Vilain.2009.FRENCH.720p.BluRay.x264-FHD.mkv", false, false)),
            new TestResult(true, new MovieInfos("Les Apprentis", null, @"\Les Apprentis\Les Apprentis.avi", false, false)),
            new TestResult(true, new MovieInfos("Liberte", null, @"\Liberte.FRENCH.DVDRip.XviD-AYMO\aymo-liberte.avi", false, false)),
            new TestResult(true, new MovieInfos("Lock, Stock and Two Smoking Barrels", 1998, @"\Lock, Stock and Two Smoking Barrels 1998 BDRip 1080p DTS De En Fr Hu x264\Lock, Stock and Two Smoking Barrels 1998.mkv", false, false)),
            new TestResult(true, new MovieInfos("Los Abrazos Rotos", 2009, @"\Los Abrazos Rotos[2009]Espanol[DvDrip]Omifast Greek\Los Abrazos Rotos (2009) CD1.divx", false, false)),
            new TestResult(true, new MovieInfos("Los Abrazos Rotos", 2009, @"\Los Abrazos Rotos[2009]Espanol[DvDrip]Omifast Greek\Los Abrazos Rotos (2009) CD2.divx", true, false)),
            new TestResult(true, new MovieInfos("Love in the Time of Cholera", null, @"\Love.in.the.Time.of.Cholera.DVDRip.XviD-DiAMOND\dmd-lovecholera-cd1.avi", false, false)),
            new TestResult(true, new MovieInfos("Love in the Time of Cholera", null, @"\Love.in.the.Time.of.Cholera.DVDRip.XviD-DiAMOND\dmd-lovecholera-cd2.avi", true, false)),
            new TestResult(true, new MovieInfos("Machete", 2010, @"\Machete.2010.BluRay.720p.DTS.x264-CHD - Brenys\Machete.2010.BluRay.720p.DTS.x264-CHD - Brenys.mkv", false, false)),
            new TestResult(true, new MovieInfos("Menace II Society", 1993, @"\Menace.II.Society.1993.720p.BluRay.AC3.x264-CHD\Menace.II.Society.1993.720p.BluRay.AC3.x264-CHD.mkv", false, false)),
            new TestResult(true, new MovieInfos("Milk", 2008, @"\Milk.2008.720p.BluRay.DTS.x264-DON\Milk.2008.720p.BluRay.DTS.x264-DON.mkv", false, false)),
            new TestResult(true, new MovieInfos("Moon", null, @"\Moon.LIMITED.720p.BluRay.x264-HAiDEAF\haideaf-moon.mkv", false, false)),
            new TestResult(true, new MovieInfos("Moscow Belgium", null, @"\Moscow Belgium FRENCH DVDRiP XViD-NTK.MZISYS.avi", false, false)),
            new TestResult(true, new MovieInfos("Mr Nobody", 2009, @"\Mr Nobody 2009 Extended 720p BluRay x264-CiNEFiLE\Mr.Nobody.2009.Extended.720p.BluRay.x264-CiNEFiLE.mkv", false, false)),
            new TestResult(true, new MovieInfos("Public Enemies", null, @"\Public.Enemies.720p.BluRay.x264-METiS\m-pe-720p[dupedb.com].mkv", false, false)),
            new TestResult(true, new MovieInfos("Rapt", null, @"\Rapt.FRENCH.BDRip.XviD-Belvaux\belvaux_rapt_xvid-cd1.avi", false, false)),
            new TestResult(true, new MovieInfos("Rapt", null, @"\Rapt.FRENCH.BDRip.XviD-Belvaux\belvaux_rapt_xvid-cd2.avi", true, false)),
            new TestResult(true, new MovieInfos("Remember Me", 2010, @"\Remember Me (2010) DVDRip XviD-MAXSPEED\Remember Me (2010) DVDRip XviD-MAXSPEED www.torentz.3xforum.ro.avi", false, false)),
            new TestResult(true, new MovieInfos("Revolutionary Road", null, @"\Revolutionary.Road.DVDRip.XviD-NeDiVx\nedivx-rroad.avi", false, false)),
            new TestResult(true, new MovieInfos("Robin Hood", 2010, @"\Robin.Hood.2010.Unrated.DC.720p.BluRay.X264-AMIABLE.[UsaBit.com]\UsaBit.com_Robin.Hood.2010.Unrated.DC.720p.BluRay.X264-AMIABLE.mkv", false, false)),
            new TestResult(true, new MovieInfos("Rocky I", 1976, @"\Rocky I(1976) BRRip(1088x576).x264.GokU61\Rocky I(1976) BRRip(1088x576).x264.GokU61.mp4", false, false)),
            new TestResult(true, new MovieInfos("Seven Pounds", null, @"\Seven.Pounds.DVDRip.XViD-PUKKA\p-7p-cd1.avi", false, false)),
            new TestResult(true, new MovieInfos("Seven Pounds", null, @"\Seven.Pounds.DVDRip.XViD-PUKKA\p-7p-cd2.avi", true, false)),
            new TestResult(true, new MovieInfos("Shawn of the Dead", null, @"\Shawn.of.the.Dead.DivX5.avi", false, false)),
            new TestResult(true, new MovieInfos("Shinjuku Incident", 2009, @"\Shinjuku.Incident.2009.720P.BDRip.X264-TLF\tlf-shinjukuincident.720bd.mkv", false, false)),
            new TestResult(true, new MovieInfos("Shutter Island", 2010, @"\Shutter.Island.2010.720p.BluRay.x264.DTS-WiKi\Shutter.Island.2010.720p.BluRay.x264.DTS-WiKi.mkv", false, false)),
            new TestResult(true, new MovieInfos("sin nombre", 2009, @"\Sin.Nombre.2009.Limited.READNFO.720p.Bluray.x264-hV\sin.nombre.2009.limited.720p.bluray.x264-hv.mkv", false, false)),
            new TestResult(true, new MovieInfos("Snatch", 2000, @"\Snatch.2000.PROPER.720p.BluRay.x264-BestHD\besthd-snatch-720p-proper.mkv", false, false)),
            new TestResult(true, new MovieInfos("State of Play", null, @"\State.of.Play.720p.BluRay.x264-REFiNED\refined-state.of.play-720p.mkv", false, false)),
            new TestResult(true, new MovieInfos("Strange Days", 1995, @"\Strange Days 1995 [HD-Rip ITA ENG - Sub Ita ~ 720p][HDitaly]\Strange Days 1995 [HD-Rip ITA ENG - Sub Ita ~ 720p][HDitaly].mkv", false, false)),
            new TestResult(true, new MovieInfos("Submarino", 2010, @"\Submarino.2010.480p.BRRip.XviD.AC3-ViSiON\Submarino.avi", false, false)),
            new TestResult(true, new MovieInfos("Terminator 3", 2003, @"\Terminator.3.2003.720p.HDDVD.x264-ESiR\Terminator.3.2003.720p.HDDVD.x264-ESiR.mkv", false, false)),
            new TestResult(false, new MovieInfos("The Boondock Saints", null, @"\The Boondock Saints 1999 BRRip H264 5.1 ch-SecretMyth (Kingdom-Release)\Audio 2 Ch\The Boondock Saints - 2 Ch.mp4", false, true)),
            new TestResult(true, new MovieInfos("The Boondock Saints", 1999, @"\The Boondock Saints 1999 BRRip H264 5.1 ch-SecretMyth (Kingdom-Release)\The Boondock Saints.mp4", false, false)),
            new TestResult(true, new MovieInfos("The Boondock Saints II All Saints Day", null, @"\The Boondock.Saints.II.All.Saints.Day.BRRIP.MP4.x264.720p-PT\The Boondock Saints II - All Saints Day (HD).m4v", false, false)),
            new TestResult(true, new MovieInfos("The Imaginarium of Doctor Parnassus", null, @"\The Imaginarium of Doctor Parnassus 720p BluRay x264-ALLiANCE\alli-doctor-720p.mkv", false, false)),
            new TestResult(true, new MovieInfos("The Visitor", 2007, @"\The Visitor[2007]DvDrip[Eng]-FXG\The Visitor[2007]DvDrip[Eng]-FXG.avi", false, false)),
            new TestResult(true, new MovieInfos("The Dark Knight", 2008, @"\The.Dark.Knight.2008.720p.nHD.x264-NhaNc3\The.Dark.Knight.2008.720p.nHD.x264-NhaNc3.mkv", false, false)),
            new TestResult(true, new MovieInfos("The Hangover", 2009, @"\The.Hang☺ver.2oo9.(720p).Blu-Ray\The.Hangover.2009.(720p).Blu-Ray SHADOW-HULK.mkv", false, false)),
            new TestResult(true, new MovieInfos("The Hangover", null, @"\The.Hangover.UNRATED.720p.BluRay.x264-REFiNED\refined-the.hangover-720p.mkv", false, false)),
            new TestResult(true, new MovieInfos("The Hurt Locker", 2008, @"\The.Hurt.Locker.2008.720p.BluRay.x264-CiRCLE\circle-thl.720p.mkv", false, false)),
            new TestResult(true, new MovieInfos("The Man From Earth", 2007, @"\The.Man.From.Earth.2007.720p.BluRay.x264-CiNEFiLE\The.Man.From.Earth.2007.720p.BluRay.x264-CiNEFiLE.mkv", false, false)),
            new TestResult(true, new MovieInfos("Tokyo!", 2008, @"\Tokyo!.2008.DVDRip.XviD.AC3.iNT-REMOVE\Tokyo!.2008.DVDRip.XviD.AC3.iNT-REMOVE.avi", false, false)),
            new TestResult(true, new MovieInfos("Treed Murray", 2001, @"\Treed.Murray (2001).avi", false, false)),
            new TestResult(true, new MovieInfos("Two Lovers", 2008, @"\Two.Lovers.2008.RoSubbed.LIMITED.720p.BluRay.x264-OUTWiTT\twolovers720p.outwitt.mkv", false, false)),
            new TestResult(true, new MovieInfos("Valkyrie", 2008, @"\Valkyrie[2008]DvDrip[Eng]-FXG\Valkyrie[2008]DvDrip[Eng]-FXG.avi", false, false)),
            new TestResult(true, new MovieInfos("Vicky Cristina Barcelona", 2008, @"\Vicky.Cristina.Barcelona[2008]DvDrip-aXXo\Vicky.Cristina.Barcelona[2008]DvDrip-aXXo .avi", false, false)),
            new TestResult(true, new MovieInfos("Waltz With Bashir", 2008, @"\Waltz.With.Bashir.2008.720p.BluRay.x264-CiNEFiLE\Waltz.With.Bashir.2008.720p.BluRay.x264-CiNEFiLE.mkv", false, false)),
            new TestResult(true, new MovieInfos("Where The Wild Things Are", null, @"\Where.The.Wild.Things.Are.720p.Bluray.x264-HUBRIS\wtwta-hubris.mkv", false, false)),
            new TestResult(false, new MovieInfos("bbs cnscg com 处刑人", null, @"\处刑人￡圣城打佛\bbs.cnscg.com 处刑人.mkv", false, false)),
        };

        public TestResult[][] ScanResults = new[]{ScanResult1, ScanResult2};

        public class FileToTite
        {
            public String File { get; set; }
            public String Title { get; set; }
            public int? Year { get; set; }

            public FileToTite(String file, String title, int? year)
            {
                File = file;
                Title = title;
                Year = year;
            }
        }

        [Test]
        public void TestScanFiles()
        {
            foreach (var scanResult in ScanResults)
            {
                foreach (var expected in scanResult)
                {
                    var e = expected;
                    var result = Scanner.ParseMovieName(expected.Movie.Path);
                    if (expected.Ok)
                        AssertMovieInfosEquals(expected.Movie, result);

                } 
            }
        }

        private static void AssertMovieInfosEquals(MovieInfos a, MovieInfos b)
        {
            Assert.AreEqual(a.GuessedTitle, b.GuessedTitle, "Title of " + a.Path);
            Assert.AreEqual(a.GuessedYear, b.GuessedYear, "Year of " + a.Path);
            Assert.AreEqual(a.ShouldBeIgnored, b.ShouldBeIgnored, "ShouldBeIgnored of " + a.Path);
            Assert.AreEqual(a.Path, b.Path, "Path of " + a.Path);
            Assert.AreEqual(a.SeamsDuplicated, b.SeamsDuplicated, "SeamsDuplicated of " + a.Path);
        }
    }
}
