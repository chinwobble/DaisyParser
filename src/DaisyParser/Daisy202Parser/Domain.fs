namespace DaisyParser.Daisy202Parser

open System
// model based on 2.02 standard 
// http://www.daisy.org/z3986/specifications/daisy_202.html?q=publications/specifications/daisy_202.html#textdoc
module Domain =
  open System.Security.Cryptography
  open NodaTime

  type MultiMediaType = 
  | ``Full Audio With Title Only`` // no navigation 
  | ``Full Audio With NCC Only``
  | ``Full Audio with NCC and Partial Text`` 
  | ``Full Audio with some text``
  | ``Full Audio and full text``
  | ``Full Text And no Audio`` 
  | ``Full Text with some Audio``

  

  type MetadataWithScheme<'a,'b> = {
    Content : 'a
    Scheme : 'b
  }

  type OptionalMetaData =
  | Contributor // dc:contributor
  | Coverage // dc:coverage extent or scope of resources 
  | Description // dc:description
  | Relation //dc:relation reference to a related resource
  | Rights // dc:rights
  | Source of MetadataWithScheme<string, string option> // dc:source
  | Subject of MetadataWithScheme<string, string option> // dc:subject
  | Type of MetadataWithScheme<string, string option> // dc:type 
  | Depth of int // ncc:depth
  | Files of int // ncc:files
  | Footnotes of int //ncc:footnotes
  | Generator of string // ncc:generator content: name and version of software that generated the NCC document
  | KByteSize of double //ncc:kByteSize
  | PageSize of int // ncc:maxPageNormal positive integer or zero indicating the content of the highest normal page occuring in the DTB
  | MultimediaType of MultiMediaType // ncc:multimediaType one of the DAISY DTB categories listed in section 1.3 above: type 1 "audioOnly"; type 2 "audioNcc"; type 3 "audioPartText"; type 4 "audioFullText"; type 5 "textPartAudio"; type 6 "textNcc". (See also the DAISY structure guidelines)
  | Narrator of string // ncc:narrator
  | ProdNotes of int // ncc:prodNotes mandatory if producer notes are used 
  | ProducedDate of MetadataWithScheme<string, string option> //  ncc:producedDate  content: date of first generation of DTB
  | Revision of int  // ncc:revision content: integer describing revision number
  | RevisionDate of MetadataWithScheme<string, string option> //ncc:revisionDate
  | SetInfo of int //ncc:setInfo
//content: k of n ; for the kth medium of a DTB spanned on n distribution media
//occurrence: mandatory in multiple volume DTB´s; inclusion in single volume DTB´s is optional - recommended
//note: playback systems must also accept elements using the deprecated name "ncc:setinfo"
  | SideBars of int 
  | SourceDate of DateTime
  | SourceEdition of string 
  | SourcePublisher of string 
  | SourceRights of string 
  | SourceTitle of string 
  | HttpEquiv of string // http-equiv

  type DaisyMetadata = {
    Creator : string // dc:creator
    Date : DateTime //dc:date
    Format : string // dc:format should be Daisy 2.02
    //Identifier : MetaDataWithScheme<string, string option> //dc:identifier
    //Language : MetaDataWithScheme<string, string option> //schemes: ISO 639 language code optionally followed by a two letter country code as in ISO 3166. For Sweden: "SV" or "SV-SE", for the United Kingdom: "EN" or "EN-UK" etc.
    Publisher : string //dc:publisher
    Title : string //dc:title
    Charset : string // ncc:charset
    PageFront : int // ncc:pageFront or ncc:page-front
    PageNormal : int // ncc:pageNormal or ncc:page-normal total number of normal pages
    PageSpecial : int // ncc:prodNotes
    TocItems : int // ncc:tocItems or ncc:tocitems or ncc:TOCitems
    TotalTime : int // ncc:totalTime time in minutes

    OptionalMetaData : OptionalMetaData seq }

  type NavigationControlCentre = {
    Title : string
    Meta : DaisyMetadata }
  
  type HReference = {
    Text: string
    File: string 
    Fragment: string }

  type AllowedClasses = 
    Title 
    | Jacket
    | Front
    | TitlePage
    | CopyrightPage
    | Acknowledgments
    | Prolog
    | Introduction
    | Dedication
    | Foreword
    | Preface
    | PrintTOC
    | Part
    | Chapter
    | Section
    | Subsection
    | MinorHead
    | Bibliography
    | Glossary
    | Appendix
    | Index
    | IndexCategory


  type Heading = {
    Id: string 
    //Class additional metadata for 
    Anchor: HReference }
  
  // identifies the beginning of the page 
  type SpanClass = 
    PageFront // page-front
    | PageNormal // page-normal
    | PageSpecial // page speical
  
  type Span = {
    Id: string 
    Class: SpanClass option //additional metadata for 
    Anchor: HReference }
  
  // todo add div support

  type BodyElement =
    H1 of Heading
    | H2 of Heading //optional
    | H3 of Heading //optional
    | H4 of Heading //optional
    | H5 of Heading //optional
    | H6 of Heading //optional
    | Span of Span
    | Div
  
  type SmilMetaData = {
    TotalElapsed: int option 
    TimeInThisSmil: int option }
  
  type SmilLayout = {
    Region: string }

  //exactly one seq
  type SmilTextReference = {
    Id: string
    File: string 
    Fragment: string }

  type SmilAudioReference = {
    File: string 
    ClipStart: Duration option
    ClipEnd: Duration option
    Id: string }

  // The <par> element is a time container whose children do not form a temporal sequence; 
  // as opposed to the <seq> element they instead occur simultaneously. 
  // In other words, media objects within a <par> element are synchronized with each other.
  
  type SmilPar = {
    Id: string
    Text: SmilTextReference
    Children: SmilParChildren }

  and SmilParChildren = 
    Audio of SmilAudioReference
    | Seqs of SmilNestedSeq

  // The <seq> element is a time container whose children form a temporal sequence. 
  // In the following definition lists, the <seq> element that occurs as a child of <body>
  // is referred to as the "main" <seq>, 
  // and <seq> elements that are nested are referred to as "nested".
  and SmilNestedSeq = 
    Audio of SmilAudioReference seq
    | Par of SmilPar * SmilPar 
  
  type SmilBody = {
    Duration: Duration // 
    Par: SmilPar seq
    Seq: (SmilNestedSeq seq) Option } // used for note reference