V této složce se nachází souborové pøílohy k bakaláøské práci:  


                "Využívání .NET assembly z neøízeného C++" 
                ==========================================
                 
                     (Vojtìch Kinkor; Plzeò, 2015)


Popis složek:
=============

  CppCliBridgeGenerator/ 
      – Adresáø obsahující projekt nástroje pro IDE Microsoft Visual Studio 2010. 

  CppCliBridgeGenerator_Examples/ 
      – Adresáø obsahující pøipravené pøíklady použitelné pro otestování funkènosti nástroje.
      - V podadresáøích se vždy nachází readme soubor k jednotlivým pøíkladùm (anglicky).

  CppCliBridgeGenerator_PerformanceTests/ 
      – Adresáø obsahující nástroje v podobì použité pro testování funkènosti v kapitole 7.
      - Testy nejsou plnì automatizované - jednotlivé metody je nutné ruènì mìnit v kódu.
      - Mìøení èasu je zahrnuto v "run" skriptech (využívá PowerShell).

  CppCliBridgeGenerator_Release/ 
      – Adresáø obsahující spouštìcí soubor nástroje (tj. zkompilovaný projekt z první zmínìné složky).

  InteropMethods/ 
      – Adresáø obsahující pøíklady používané v kapitole 3. 



Popis souborù:
==============

  Kinkor_A12B0082P_BP.pdf / Kinkor_A12B0082P_BP.docx 
      – Text bakaláøské práce ve formátu PDF a DOCX.
      - Obsahuje uživatelskou pøíruèku nástroje (pøíloha A, strana 44 v PDF/39 èíslovaná). 

  readme.txt 
      – Tento soubor.



Dodateèné informace:
====================

 Ve složkách s ukázkami jsou umístìné dávkové soubory (.bat) sloužící pro rychlé vyzkoušení dané ukázky.
 Tyto soubory je nutné spouštìt ze zapisovatelného média (tedy napøíklad z pevného disku).

 Pro vyzkoušení vìtšiny ukázek je potøeba IDE Microsoft Visual Studio 2010 nebo novìjší a .NET Framework 4.

 Soubory __vcvars.bat slouží pro nastavení vývojového prostøedí. V pøípadì, že se nepodaøí automatické
 nastavení, budete upozornìni s žádostí o ruèní nastavení cesty k souboru vcvarsall.bat (který je souèástí
 IDE Visual Studio).

 Po spuštìní souboru __vcvars.bat v samostatném konzolovém oknì se zpøístupní nástroj msbuild, který 
 lze použít pro kompilaci vygenerovaných C++/CLI mostù.

