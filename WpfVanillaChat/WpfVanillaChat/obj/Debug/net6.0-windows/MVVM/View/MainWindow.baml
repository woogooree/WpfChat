   M S B A M L   `   `   `  ����I  EWpfVanillaChat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:WpfVanillaChatWpfVanillaChat  5   	   M+clr-namespace:WpfVanillaChat.MVVM.ViewModelWpfVanillaChat.MVVM.ViewModel  64       WpfVanillaChat.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:WpfVanillaChat     5   	   A	viewmodel+clr-namespace:WpfVanillaChat.MVVM.ViewModel     64   $	��650��5   	   $
��1200��6   ���#36393F  � WindowStyle$
  None=�5
   	    � AllowsTransparency$
 True��5   	    � 
ResizeMode$ CanResizeWithGrip=�5   	    � DataContext 5      1   +WpfVanillaChat.MVVM.ViewModel.MainViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $��25 �6   �� 5       � ColumnDefinitions 5   
   �� 5      $	��200 �6   �� 5      .+�5   
   �� -   5        $��2��5      ���#252525._�5      � $��Stretch=�6   .+�5      �� $��VanillaChat��6   
�������$��SemiBold"�5!      �� 5"      $��Right=�6    �� Orientation$ 
Horizontal=�5#      .+�5$      �� -   5         ��Name$ btnMinimaze��5$      $��20��62   $��20��6=   $��_��5%      
������ $��0q�5&      
�������$
��Bold"�5(      $��0,0,0,3q�5)      5*      �� 5+      -   5        $ btnWindowState��5+      $��20��65   $��20��6@   $	��[ ]��5,      
������ $��0q�5-      
�������$
��Bold"�5/      50      �� 51      -   5        $ btnClose��51      $��20��6/   $��20��6:   $��X��52      
������ $��0q�53      
�������$
��Bold"�55      56      57      58      59      � 5;   
   ���#2F3136$��1��6$    5<      �� 5=      $��50 �6    �� 5>      �� 5?      $��60 �6    .+�5B      �� $��@Channel��6   $��Center=�6'   $��SemiBold"�6B   
�������$��8,0,0,0q�6k   �� 5C      
������ $��0q�5F      $��1��6+      ContactCard#��[  5G      ��5C      �� *Users+ �� SelectedItem 5D      �� *SelectedContact+5G      �� 5I      $��2��6   .+�5J      �� $��30��6   ���#292B2F
�������$��SemiBold"�5M      ��5N      �� *
Username+	 �� UpdateSourceTrigger$	 PropertyChanged=��� 5O      $��30��6   ���#292B2F
�������$��SemiBold"�5R      $��Connect��5S      ��5T      �� *ConnectToServerCommand+5U      5V      � 5X   
   $��1��6   $��1��6    5Y      �� 5Z      $��50 �6    �� 5[      �� 5\      $��60 �6    .+�5_      �� ���#2F3136$��0,0,0,2q�6+   ._�5`      � $��Stretch=�6   $��8q�65   .+�5a      �� $��	@Username��6   
�������$
��Bold"�6C   $��5,0,5,0q�6U   $��Center=�6f   �� 5b      $ 
Horizontal=�6!   $��Right=�6:   .+�5c      �� $��20��6    $��20��6+   
 �� BitmapScalingMode$

 Fant=�67   $��5,0,5,0q�6^   $��./Icons/call.png��6o   5d      5e      5f      �� 5h      
������ $��0q�5i   0   
�������$��16(�5j   *   $��8,0,0,0q�5k      $��1��5l      ��5h      �� *Chats+5l      � 5n      $��2��6    5o      �� 5p      �� 5q      $��90 �6'   .+�5t      �� $��1��6   $��59��6'   $	��910��63   $
��Left=�6?   $��Stretch=�5u      $��Center=�5v      $��2��5y      ���#292B2F$��1q�5{      
�������$��Medium"�5}      ��5x      �� *Chat+$	 PropertyChanged=�5}      �� 5      $ 
Horizontal=�6   $��90��66   $��Right=�5�      ���#292B2F$��1��5�      .+�5�      �� $��Enter��6   $��59��5�      $��89��5�      ���#292B2F
�������$��20(�5�      ��5�      �� *SendMessageCommand+5�      5�      5�      5�      5�      