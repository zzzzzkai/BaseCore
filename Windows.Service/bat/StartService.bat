@echo.��������......  
@echo off  
@net start TimeService  
@sc config TimeService start= AUTO  
@echo off  
@echo.������ϣ�  
@pause