using Bogus;
using DevBook.API.Features.BookStore.ProductCategories;
using DevBook.API.Features.BookStore.Products;
using DevBook.API.Features.BookStore.Products.Books;

namespace DevBook.API.Features.BookStore;

internal sealed class BookStoreDataSeeder
{
	private readonly Random random = new();
	private const int MaxBookDataCound = 50;

	public async Task<ProductCategory[]> SeedCategories(DevBookDbContext dbContext)
	{
		//Set the randomizer seed to generate repeatable data sets.
		Randomizer.Seed = random;

		ProductCategory[] subcategories = [
			new ProductCategory
			{
				Name = "Sci-fi",
				IsTopLevelCategory = false,
				Subcategories = [],
			},
			new ProductCategory
			{
				Name = "Romance",
				IsTopLevelCategory = false,
				Subcategories = [],
			},
		];

		List<ProductCategory> topLevelCategories = [
			new ProductCategory
			{
				Name = "Books",
				IsTopLevelCategory = true,
				Subcategories = subcategories.Select(x => x.Id).ToList(),
			}
		];

		await dbContext.AddRangeAsync(subcategories);
		await dbContext.AddRangeAsync(topLevelCategories);

		return [.. subcategories, .. topLevelCategories];
	}

	public async Task SeedBooks(DevBookDbContext dbContext, ProductCategory[] productCategories)
	{
		//Set the randomizer seed to generate repeatable data sets.
		Randomizer.Seed = random;
		int bookFakerIndex = 0;

		var books = new Faker<Book>()
			.RuleFor(x => x.Name, () => GetBookData(bookFakerIndex).name)
			.RuleFor(x => x.ProductType, ProductType.Book)
			.RuleFor(x => x.RetailPrice, f => f.Random.Number(1, 100))
			.RuleFor(x => x.Price, f => f.Random.Number(50, 150))
			.RuleFor(x => x.DiscountAmmount, f => f.Random.Number(0, 1))

			.RuleFor(x => x.Author, () => GetBookData(bookFakerIndex).author)
			.RuleFor(x => x.Description, () => GetBookData(bookFakerIndex).description)
			.RuleFor(x => x.ProductCategoryIds, f => productCategories
				.Skip(f.Random.Number(0, productCategories.Length - 1))
				.Take(f.Random.Number(0, productCategories.Length - 1))
				.Select(x => x.Id)
				.ToList())
			.FinishWith((f, x) => bookFakerIndex += 1)
			.Generate(MaxBookDataCound);

		await dbContext.AddRangeAsync(books);
	}

	private (string name, string author, string description) GetBookData(int bookIndex)
	{
		if (bookIndex < 0 || bookIndex > BookData.Length)
		{
			throw new IndexOutOfRangeException($"Invalid bookIndex value '{bookIndex}', only indexes between 0 and {BookData.Length} are allowed.");
		}

		return BookData[bookIndex];
	}

	private readonly (string name, string author, string description)[] BookData = [
		new ("Don Quixote", "Miguel de Cervantes", "A novel that follows the adventures of a delusional nobleman who believes he is a knight. Along with his loyal squire, Sancho Panza, he embarks on a series of absurd yet heroic exploits."),
		new ("Alice's Adventures in Wonderland", "Lewis Carroll", "Alice falls through a rabbit hole into a fantasy world where she encounters bizarre creatures and whimsical adventures. A classic of children's literature."),
		new ("The Adventures of Huckleberry Finn", "Mark Twain", "Huckleberry Finn travels down the Mississippi River with a runaway slave, Jim. The novel explores themes of freedom, race, and morality."),
		new ("The Adventures of Tom Sawyer", "Mark Twain", "Tom Sawyer, a mischievous boy, embarks on adventures with his friends, including witnessing a murder and searching for hidden treasure."),
		new ("Treasure Island", "Robert Louis Stevenson", "A young boy named Jim Hawkins discovers a treasure map and embarks on a dangerous journey to find the treasure. Filled with pirates, mutiny, and adventure."),
		new ("Pride and Prejudice", "Jane Austen", "Elizabeth Bennet navigates love, society, and family in 19th-century England. The novel explores issues of class, marriage, and morality."),
		new ("Wuthering Heights", "Emily Brontë", "A dark and passionate tale of love and revenge set on the Yorkshire moors. The relationship between Heathcliff and Catherine Earnshaw is central to the novel's tragedy."),
		new ("Jane Eyre", "Charlotte Brontë", "An orphaned girl grows up to become a governess and falls in love with her employer, Mr. Rochester. The novel explores themes of independence, morality, and love."),
		new ("Moby Dick", "Herman Melville", "Captain Ahab is obsessed with hunting the white whale, Moby Dick, at all costs. The novel explores themes of fate, obsession, and the conflict between man and nature."),
		new ("The Scarlet Letter", "Nathaniel Hawthorne", "A woman in Puritan New England is shunned for bearing an illegitimate child. The novel explores guilt, sin, and redemption."),
		new ("Gulliver's Travels", "Jonathan Swift", "Lemuel Gulliver embarks on a series of fantastical voyages to strange lands, including Lilliput and Brobdingnag. A satire of human nature and society."),
		new ("The Pilgrim's Progress", "John Bunyan", "An allegory of a Christian's journey through life, following the protagonist, Christian, as he navigates spiritual trials and challenges."),
		new ("A Christmas Carol", "Charles Dickens", "Ebenezer Scrooge, a miserly old man, is visited by three ghosts on Christmas Eve, leading him to a transformation in his outlook on life and generosity."),
		new ("David Copperfield", "Charles Dickens", "The semi-autobiographical story of David Copperfield's journey from an impoverished boy to a successful author. It explores themes of hardship, resilience, and personal growth."),
		new ("A Tale of Two Cities", "Charles Dickens", "Set in London and Paris during the French Revolution, the novel follows the lives of several characters, including Charles Darnay and Sydney Carton, amidst political turmoil."),
		new ("Little Women", "Louisa May Alcott", "The story of four sisters—Meg, Jo, Beth, and Amy—growing up during the American Civil War. The novel explores family, love, and personal aspirations."),
		new ("Great Expectations", "Charles Dickens", "Pip, an orphan, aspires to improve his social standing after receiving a mysterious fortune. The novel explores themes of ambition, social class, and personal growth."),
		new ("The Hobbit, or, There and Back Again", "J.R.R. Tolkien", "Bilbo Baggins, a reluctant hobbit, embarks on a dangerous adventure with a group of dwarves to reclaim a stolen treasure from a dragon."),
		new ("Frankenstein, or, the Modern Prometheus", "Mary Shelley", "Victor Frankenstein creates a living being from dead body parts, only to be horrified by his creation. The novel explores themes of ambition, guilt, and the consequences of scientific experimentation."),
		new ("Oliver Twist", "Charles Dickens", "An orphan boy named Oliver escapes a life of poverty and crime in Victorian England. The novel highlights the harsh conditions faced by the poor."),
		new ("Uncle Tom's Cabin", "Harriet Beecher Stowe", "A powerful anti-slavery novel that depicts the harsh realities of slavery in the American South. It helped fuel the abolitionist movement."),
		new ("Crime and Punishment", "Fyodor Dostoyevsky", "Raskolnikov, a poor student, commits a murder and struggles with guilt and morality in this psychological exploration of crime, punishment, and redemption."),
		new ("Madame Bovary: Patterns of Provincial life", "Gustave Flaubert", "Emma Bovary, dissatisfied with her provincial life and marriage, seeks excitement in adulterous affairs, leading to her downfall."),
		new ("The Return of the King", "J.R.R. Tolkien", "The final installment of 'The Lord of the Rings' trilogy. It chronicles the defeat of Sauron and the restoration of Aragorn to the throne."),
		new ("Dracula", "Bram Stoker", "The story of Count Dracula's attempt to move from Transylvania to England in search of new blood, and the group of people determined to stop him."),
		new ("The Three Musketeers", "Alexandre Dumas", "D'Artagnan joins the Three Musketeers—Athos, Porthos, and Aramis—in their adventures as they protect the king and fight intrigue in 17th-century France."),
		new ("Brave New World", "Aldous Huxley", "A dystopian novel set in a future society that sacrifices individuality for the sake of order and stability. It explores the consequences of state control and technological advancement."),
		new ("War and Peace", "Leo Tolstoy", "An epic tale set against the backdrop of the Napoleonic wars, following the lives of several aristocratic families in Russia."),
		new ("To Kill a Mockingbird", "Harper Lee", "A young girl, Scout, grows up in the racially charged American South, where her father defends a black man wrongly accused of raping a white woman."),
		new ("The Wizard of Oz", "L. Frank Baum", "Dorothy and her dog Toto are swept away to the magical land of Oz, where they embark on a journey to meet the Wizard and return home."),
		new ("Les Misérables", "Victor Hugo", "The novel follows the lives of several characters, including ex-convict Jean Valjean, in the aftermath of the French Revolution, exploring themes of justice, love, and redemption."),
		new ("The Secret Garden", "Frances Hodgson Burnett", "A young orphan, Mary Lennox, discovers a hidden, neglected garden and works to restore it. The novel emphasizes the healing power of nature and friendship."),
		new ("Animal Farm", "George Orwell", "A political allegory where farm animals overthrow their human owner to establish a society, only for it to become corrupt under the leadership of the pigs."),
		new ("The Great Gatsby", "F. Scott Fitzgerald", "A novel about the mysterious millionaire Jay Gatsby and his obsession with Daisy Buchanan. It explores themes of wealth, love, and the American Dream."),
		new ("The Little Prince", "Antoine de Saint-Exupéry", "A young prince travels from planet to planet, learning life lessons about love, loss, and human nature."),
		new ("The Call of the Wild", "Jack London", "A domesticated dog named Buck is stolen from his home and thrust into the wilds of Alaska during the Klondike Gold Rush, where he must learn to survive."),
		new ("20,000 Leagues Under the Sea", "Jules Verne", "Captain Nemo takes his guests on a journey aboard the submarine Nautilus, exploring the wonders of the ocean while hiding from society."),
		new ("Anna Karenina", "Leo Tolstoy", "A tragic love story about a Russian aristocrat, Anna, who has an affair that leads to her downfall. It explores themes of love, family, and society."),
		new ("The Wind in the Willows", "Kenneth Grahame", "A tale of friendship and adventure among animals living along the riverbank, featuring characters like Mole, Rat, Toad, and Badger."),
		new ("The Picture of Dorian Gray", "Oscar Wilde", "Dorian Gray remains eternally youthful while his portrait ages, reflecting the consequences of his immoral lifestyle."),
		new ("The Grapes of Wrath", "John Steinbeck", "The Joad family, displaced by the Dust Bowl, travels to California in search of a better life, only to face further hardship and injustice."),
		new ("Sense and Sensibility", "Jane Austen", "Two sisters, Elinor and Marianne, navigate love and heartbreak in 19th-century England. The novel explores the balance between emotion and reason."),
		new ("The Last of the Mohicans", "James Fenimore Cooper", "Set during the French and Indian War, this novel follows the adventures of Hawkeye and his Mohican friends as they navigate the dangers of frontier life."),
		new ("Tess of the d'Urbervilles", "Thomas Hardy", "A tragic story of a young woman, Tess, who is betrayed by the men in her life and society, leading to her downfall."),
		new ("Harry Potter and the Sorcerer's Stone", "J.K. Rowling", "An orphaned boy, Harry Potter, discovers he is a wizard and attends Hogwarts School of Witchcraft and Wizardry. The first in a beloved fantasy series."),
		new ("Heidi", "Johanna Spyri", "A young girl named Heidi is sent to live with her grandfather in the Swiss Alps, where she grows to love the mountains and brings joy to those around her."),
		new ("Ulysses", "James Joyce", "A modern retelling of Homer's Odyssey, set in a single day in Dublin, as Leopold Bloom navigates various episodes that mirror the ancient epic."),
		new ("The Complete Sherlock Holmes", "Arthur Conan Doyle", "A collection of stories featuring Sherlock Holmes, the iconic detective, and his companion Dr. Watson as they solve various mysteries."),
		new ("The Count of Monte Cristo", "Alexandre Dumas", "Edmond Dantès is wrongfully imprisoned and later escapes, seeking revenge on those who betrayed him. A tale of justice, revenge, and redemption."),
		new ("The Old Man and the Sea", "Ernest Hemingway", "An aging fisherman, Santiago, engages in an epic struggle to catch a giant marlin, reflecting themes of perseverance and man's battle against nature.")
	];
}
