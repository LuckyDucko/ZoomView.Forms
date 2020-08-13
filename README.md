
<br />
<p align="center">
  <h1 align="center">ZoomView.Forms</h3>
  <p align="center">
  Quickly add a zoomable wrapper to any VisualElement
    <br />
  </p>
</p>


<!-- TABLE OF CONTENTS -->

## Table of Contents

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/ae337590962b4e28955d14f745f93b13)](https://www.codacy.com/manual/LuckyDucko/ZoomView.Forms?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=LuckyDucko/ZoomView.Forms&amp;utm_campaign=Badge_Grade)
[![nuget](https://img.shields.io/nuget/v/ZoomView.Forms.svg)](https://www.nuget.org/packages/ZoomView.Forms)

* [Getting Started](#getting-started)
  * [Installation](#installation)
* [Usage](#usage)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)


<!-- ABOUT THE PROJECT -->
## About The Project

ZoomView.Forms aims to provide iOS and Android Xamarin Developer the ability to add a 'zoomview', which is basically a scrollview with the ability to pan in/out.
The gesture recognition also is able to scale with your content.


<!-- GETTING STARTED -->
## Getting Started

Simply add `xmlns:ZoomView="clr-namespace:ZoomView.Forms;assembly=ZoomView.Forms.Plugin"` to your xaml pages
then all you need is 
```
<ZoomView:ZoomView>
  Your view here
</ZoomView:ZoomView>
```

### Installation
Ensure that ZoomView.Forms is added to all projects, shared and Android/iOS

<!-- USAGE EXAMPLES -->
## Usage

**NOTE: Zooming can be interfered with by other touches being registered at the beginning of the pan gesture.**

here is an example view that i created
```XML
<ZoomView:ZoomView>
		<StackLayout Margin="20,0,20,0" VerticalOptions="Center">
			<Label Text="Phone number" TextColor="WhiteSmoke" />
			<Entry
            IsSpellCheckEnabled="false"
            IsTextPredictionEnabled="false"
            Keyboard="Telephone"
            MaxLength="16"
            Text="{Binding Mobile, Mode=TwoWay}" />
			<Label
            Margin="0,15,0,0"
            Text="PIN"
            TextColor="WhiteSmoke" />
			<Entry
            IsPassword="true"
            IsSpellCheckEnabled="false"
            IsTextPredictionEnabled="false"
            Text="{Binding Password, Mode=TwoWay}" />
			<Button
            Margin="0,25,0,20"
            BackgroundColor="Green"
            BorderColor="Transparent"
            BorderWidth="1"
            Command="{Binding LoginAsync}"
            CornerRadius="25"
            FontAttributes="Bold"
            HeightRequest="50"
            IsEnabled="{Binding LoginEnabled}"
            Text="Sign In"
            TextColor="White" />
		</StackLayout>
	</ZoomView:ZoomView>
```
**Result:**


![iOS Gif Example](https://j.gifs.com/q74W92.gif)
![Android Gif Example](https://j.gifs.com/oV4WPj.gif)


<!-- LICENSE -->
## License

This project uses the MIT License

<!-- CONTACT -->
## Contact

My [Github](https://github.com/LuckyDucko),
or reach me on the [Xamarin Slack](https://xamarinchat.herokuapp.com/),
or on my [E-mail](tyson@logchecker.com.au)

Project Link: [ZoomView.Forms](https://github.com/LuckyDucko/ZoomView.Forms)


<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* The Android code i got my main base from one of the [following](https://stackoverflow.com/search?q=zoomable+android+view&s=f3c43e0e-79a6-4529-a3ea-adf8f8f3ab14)
I will continue searching for the exact one. however was a while ago.
