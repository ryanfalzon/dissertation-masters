<!-- PROJECT SHIELDS -->
[![LinkedIn][linkedin-shield]][linkedin-url]



<!-- FRONT PAGE-->
<br />
<p align="center">
  <a href="https://www.um.edu.mt/">
    <img src="UoM.png" alt="Logo" width="100" height="100">
  </a>

  <h2 align="center">A Unified Approach to Distributed Application Development for DLT</h2>
  <h3 align="center">Ryan Falzon</h3>
  <h4 align="center">Supervised by Prof. Gordon Pace</h4>
  <h4 align="center">Co-supervised by Dr Joshua Ellul</h4>
  <h5 align="center">Centre for Distributed Ledger Technologies</h5>
  <h5 align="center">University of Malta</h5>
  <h6 align="center">April 2021</h6>

<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#built-with">Built With</a></li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>



<!-- BUILT WITH -->
## Built With

* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)



<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

1.  Download .NET Core 3.1 from [here](https://dotnet.microsoft.com/download).
2.  Download Git from [here](https://git-scm.com/downloads).

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/ryanfalzon/DLT-Dissertation
   ```
2. Build the `UnifiedModel.SourceGenerator` project
   ```sh
   cd Dissertation/src/UnifiedModel/UnifiedModel.SourceGenerator
   dotnet build --output "c:/UnifiedModel/SourceGenerator"
   ```
3. Build the `UnifiedModel.Connectors` project
   ```sh
   cd Dissertation/src/UnifiedModel/UnifiedModel.Connectors
   dotnet build --output "c:/UnifiedModel/Connectors"
   ```
4. Run the `UnifiedModel.SourceGenerator.exe` executable
   ```sh
   UnifiedModel.SourceGenerator.exe "UnifiedCode.txt" "OutputDirectory"
   ```
5. Once generated smart contracts are deployed, fill in the `Ethereum.json` file created in the output directory of step 3
   ```JSON
   {
    "PublicKey": "",
    "PrivateKey": "",
    "Contracts": [
      {
        "Name": "",
        "AbiLocation": "",
        "Address": ""
      }
    ]
   }
   ```
6. Copy the contents of the output directory of step 3 to the location where the desktop code will be running



<!-- USAGE EXAMPLES -->
## Framewok Usage

This is an example of how the framework can be used to create annotated code to be compiled down to C# and Solidity code for Desktop and Etheruem enviornments.
```csharp
[XOn(All)]
public class Profile : XModel("SocialNetwork")
{
  ...
}

[XOn(Ethereum)]
public class SocialNetwork
{
  ...
}
  
[XOn("All")]
public class SocialNetwork
{
  public void Register(Profile profile)
  {
    @XOn("Desktop", profile)
    {
      ...
    }

    ~@XOn("Ethereum", profile)
    {
      ...
    }
  }
}
```



<!-- CONTACT -->
## Contact

[Ryan Falzon](https://www.linkedin.com/in/ryan-falzon-291a3516a/) - rfalzonryan@gmail.com



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
I would like to thank my supervisor Prof. Gordon Pace and co-supervisor Dr. Joshua Ellul for their continual support and guidance throughout this research study. Additionally, I would like to express my gratitude to all lecturers of the Centre of Distributed Ledger Technologies for the knowledge they imparted on throughout their lectures.



<!-- MARKDOWN LINKS & IMAGES -->
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/ryan-falzon-291a3516a/
